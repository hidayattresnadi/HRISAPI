using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Process;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Mail;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using LibrarySystem.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Security.Claims;

namespace HRISAPI.Application.Services
{
    public class ProcessService : IProcessService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWorkflowActionRepository _workflowActionRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IWorkflowSequenceRepository _workflowSequenceRepository;
        private readonly INextStepRulesRepository _nextStepRulesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        public ProcessService(IRequestRepository requestRepository, IProcessRepository processRepository,
                           IWorkflowActionRepository workflowActionRepository, IHttpContextAccessor httpContextAccessor, IWorkflowSequenceRepository workflowSequenceRepository,
                           RoleManager<IdentityRole> roleManager, INextStepRulesRepository nextStepRulesRepository, IEmailService emailService, UserManager<AppUser> userManager,
                           IEmployeeRepository employeeRepository, ILeaveRequestRepository leaveRequestRepository)
        {
            _requestRepository = requestRepository;
            _processRepository = processRepository;
            _workflowActionRepository = workflowActionRepository;
            _httpContextAccessor = httpContextAccessor;
            _workflowSequenceRepository = workflowSequenceRepository;
            _roleManager = roleManager;
            _nextStepRulesRepository = nextStepRulesRepository;
            _emailService = emailService;
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }
        private string ReplacePlaceholders(string template, AppUser foundEmployee, LeaveRequest leaveRequest, string name, string comment)
        {
            template = template.Replace("{{EmployeeId}}", foundEmployee.EmployeeId.ToString());
            template = template.Replace("{{StartDate}}", leaveRequest.StartDate.ToString("d MMMM yyyy", new CultureInfo("id-ID")));
            template = template.Replace("{{EndDate}}", leaveRequest.EndDate.ToString("d MMMM yyyy", new CultureInfo("id-ID")));
            template = template.Replace("{{Name}}", name);
            template = template.Replace("{{Leave Type}}", leaveRequest.LeaveType);
            template = template.Replace("{{Reason}}", leaveRequest.Reason);
            template = template.Replace("{{Comment}}", comment);

            return template;
        }
        public async Task<Response> ReviewRequest(int processId, ProcessDTOApproved requestApproval)
        {
            var userRoleIds = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == "RoleId")
                .Select(c => c.Value)
                .ToList();
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var process = await _processRepository.GetFirstOrDefaultAsync(p => p.ProcessId == processId);
            var workflowSequence = await _workflowSequenceRepository.GetFirstOrDefaultAsync(wfs => wfs.StepId == process.CurrentStepId);
            bool isValidRole = userRoleIds.Contains(workflowSequence.RequiredRole);
            var role = await _roleManager.FindByIdAsync(workflowSequence.RequiredRole);
            if (!isValidRole)
            {
                return new Response
                {
                    Status = "Error",
                    Message = "Role is not valid"
                };
            }
            
            if (requestApproval.RequestStatus == "Request Approved" && role.Name == Roles.Role_Employee_Supervisor)
            {
                var foundEmployee = await _userManager.FindByIdAsync(process.RequesterId);
                var foundEmployeeData = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == foundEmployee.EmployeeId, "Supervisor");

                var currentEmployeeLoggedinId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
                int? intEmployeeId = string.IsNullOrEmpty(currentEmployeeLoggedinId) ? (int?)null : int.Parse(currentEmployeeLoggedinId);

                if (foundEmployeeData.Supervisor.EmployeeId != intEmployeeId)
                {
                    return new Response
                    {
                        Status = "Error",
                        Message = "You are not the supervisor of this employee"
                    };
                }
                var nextStepId = await _nextStepRulesRepository.GetFirstOrDefaultAsync(n => n.CurrentStepId == process.CurrentStepId);
                process.CurrentStepId = nextStepId.NextStepId;
                process.Status = "Pending HR Manager Review";
                await _processRepository.SaveAsync();

                var newWorkflowAction = new WorkflowAction
                {
                    ProcessId = process.ProcessId,
                    StepId = nextStepId.CurrentStepId,
                    ActorId = userId,
                    Action = "Request Approved",
                    ActionDate = DateTime.UtcNow,
                    Comments = requestApproval.Comment
                };
                await _workflowActionRepository.AddAsync(newWorkflowAction);
                await _workflowActionRepository.SaveAsync();

                var leaveRequest = await _leaveRequestRepository.GetFirstOrDefaultAsync(l => l.ProcessId == process.ProcessId);

                var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/ApprovedLeaveRequest.html");
                htmlTemplate = ReplacePlaceholders(htmlTemplate, foundEmployee, leaveRequest, foundEmployeeData.Supervisor.EmployeeName,requestApproval.Comment);

                var mailData = new MailData
                {
                    EmailToName = foundEmployeeData.EmployeeName,
                    EmailSubject = "Leave request is approved by Supervisor",
                };

                mailData.EmailToIds.Add(foundEmployeeData.Supervisor.EmailAddress);
                mailData.EmailToIds.Add(foundEmployeeData.EmailAddress);
                mailData.EmailBody = htmlTemplate;

                var emailResult = await _emailService.SendEmailAsync(mailData);

                var workflowSequenceNext = await _workflowSequenceRepository.GetFirstOrDefaultAsync(wfs => wfs.StepId == nextStepId.NextStepId);
                var nextRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == workflowSequenceNext.RequiredRole);
                var usersInRole = await _userManager.GetUsersInRoleAsync(nextRole.Name);
                var user = usersInRole.FirstOrDefault();
                var emailHRManager = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == user.EmployeeId);

                var htmlTemplateHRManager = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/RequestApprovedToHR.html");
                htmlTemplateHRManager = ReplacePlaceholders(htmlTemplateHRManager, foundEmployee, leaveRequest, user.UserName,requestApproval.Comment);

                var mailDataHRManager = new MailData
                {
                    EmailToName = user.UserName,
                    EmailSubject = "Leave request is approved by Supervisor",
                };

                mailDataHRManager.EmailToIds.Add(emailHRManager.EmailAddress);
                mailDataHRManager.EmailBody = htmlTemplateHRManager;
                await _emailService.SendEmailAsync(mailDataHRManager);
            }
            else if (requestApproval.RequestStatus == "Request Approved" && role.Name == Roles.Role_HR_Manager)
            {
                var foundEmployee = await _userManager.FindByIdAsync(process.RequesterId);
                var foundEmployeeData = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == foundEmployee.EmployeeId, "Supervisor");
                var nextStepId = await _nextStepRulesRepository.GetFirstOrDefaultAsync(n => n.CurrentStepId == process.CurrentStepId && n.ConditionValue == "Approved");
                var newWorkflowAction = new WorkflowAction
                {
                    ProcessId = process.ProcessId,
                    StepId = process.CurrentStepId,
                    ActorId = userId,
                    Action = "Request Approved",
                    ActionDate = DateTime.UtcNow,
                    Comments = requestApproval.Comment
                };
                await _workflowActionRepository.AddAsync(newWorkflowAction);
                await _workflowActionRepository.SaveAsync();

                process.CurrentStepId = nextStepId.NextStepId;
                process.Status = "Accepted";
                await _processRepository.SaveAsync();

                var newWorkflowActionAccepted = new WorkflowAction
                {
                    ProcessId = process.ProcessId,
                    StepId = process.CurrentStepId,
                    ActorId = userId,
                    Action = "Request Approved",
                    ActionDate = DateTime.UtcNow,
                    Comments = requestApproval.Comment
                };
                await _workflowActionRepository.AddAsync(newWorkflowActionAccepted);
                await _workflowActionRepository.SaveAsync();

                var request = await _requestRepository.GetFirstOrDefaultAsync(r => r.RequestId == process.RequestId);
                request.EndDate = DateTime.UtcNow;
                await _requestRepository.SaveAsync();

                var leaveRequest = await _leaveRequestRepository.GetFirstOrDefaultAsync(l => l.ProcessId == process.ProcessId);
                var emailHRManager = await _userManager.FindByIdAsync(userId);
                var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/ApprovedLeaveRequestHR.html");
                htmlTemplate = ReplacePlaceholders(htmlTemplate, foundEmployee, leaveRequest, foundEmployeeData.EmployeeName, requestApproval.Comment);

                var mailData = new MailData
                {
                    EmailToName = foundEmployeeData.EmployeeName,
                    EmailSubject = "Leave request is approved by Supervisor",
                };

                mailData.EmailToIds.Add(foundEmployeeData.Supervisor.EmailAddress);
                mailData.EmailToIds.Add(foundEmployeeData.EmailAddress);
                mailData.EmailToIds.Add(emailHRManager.Email);
                mailData.EmailBody = htmlTemplate;

                var emailResult = _emailService.SendEmailAsync(mailData);
            }
            else if (requestApproval.RequestStatus == "Request Rejected")
            {
                if(role.Name == Roles.Role_Employee_Supervisor)
                {
                    var foundEmployee = await _userManager.FindByIdAsync(process.RequesterId);
                    var foundEmployeeData = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == foundEmployee.EmployeeId, "Supervisor");

                    var currentEmployeeLoggedinId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
                    int? intEmployeeId = string.IsNullOrEmpty(currentEmployeeLoggedinId) ? (int?)null : int.Parse(currentEmployeeLoggedinId);

                    if (foundEmployeeData.Supervisor.EmployeeId != intEmployeeId)
                    {
                        return new Response
                        {
                            Status = "Error",
                            Message = "You are not the supervisor of this employee"
                        };
                    }
                    var leaveRequest = await _leaveRequestRepository.GetFirstOrDefaultAsync(l => l.ProcessId == process.ProcessId);

                    var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/RequestLeaveRejected.html");
                    htmlTemplate = ReplacePlaceholders(htmlTemplate, foundEmployee, leaveRequest, foundEmployeeData.Supervisor.EmployeeName, requestApproval.Comment);
                    htmlTemplate = htmlTemplate.Replace("{{Role}}", Roles.Role_Employee_Supervisor);

                    var mailData = new MailData
                    {
                        EmailToName = foundEmployeeData.EmployeeName,
                        EmailSubject = "Leave request is rejected by Supervisor",
                    };

                    mailData.EmailToIds.Add(foundEmployeeData.Supervisor.EmailAddress);
                    mailData.EmailToIds.Add(foundEmployeeData.EmailAddress);
                    mailData.EmailBody = htmlTemplate;

                    var emailResult = await _emailService.SendEmailAsync(mailData);
                }
                var nextStepId = await _nextStepRulesRepository.GetFirstOrDefaultAsync(n => n.CurrentStepId == process.CurrentStepId && n.ConditionValue == "Rejected");
                var newWorkflowActionRejected = new WorkflowAction
                {
                    ProcessId = process.ProcessId,
                    StepId = process.CurrentStepId,
                    ActorId = userId,
                    Action = "Request Rejected",
                    ActionDate = DateTime.UtcNow,
                    Comments = requestApproval.Comment
                };

                await _workflowActionRepository.AddAsync(newWorkflowActionRejected);
                await _workflowActionRepository.SaveAsync();

                process.Status = "Rejected";
                process.CurrentStepId = nextStepId.NextStepId;
                await _processRepository.SaveAsync();

                var newWorkflowAction = new WorkflowAction
                {
                    ProcessId = process.ProcessId,
                    StepId = process.CurrentStepId,
                    ActorId = userId,
                    Action = "Request Rejected",
                    ActionDate = DateTime.UtcNow,
                    Comments = requestApproval.Comment
                };
                await _workflowActionRepository.AddAsync(newWorkflowAction);
                await _workflowActionRepository.SaveAsync();

                var request = await _requestRepository.GetFirstOrDefaultAsync(r => r.RequestId == process.RequestId);
                request.EndDate = DateTime.UtcNow;
                await _requestRepository.SaveAsync();

                if(role.Name == Roles.Role_HR_Manager)
                {
                    var foundEmployee = await _userManager.FindByIdAsync(process.RequesterId);
                    var foundEmployeeData = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == foundEmployee.EmployeeId, "Supervisor");
                    var leaveRequest = await _leaveRequestRepository.GetFirstOrDefaultAsync(l => l.ProcessId == process.ProcessId);
                    var emailHRManager = await _userManager.FindByIdAsync(userId);
                    var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/RequestLeaveRejected.html");
                    htmlTemplate = ReplacePlaceholders(htmlTemplate, foundEmployee, leaveRequest, foundEmployeeData.EmployeeName, requestApproval.Comment);
                    htmlTemplate = htmlTemplate.Replace("{{Role}}", Roles.Role_HR_Manager);

                    var mailData = new MailData
                    {
                        EmailToName = foundEmployeeData.EmployeeName,
                        EmailSubject = "Leave request is rejected by HR Manager",
                    };

                    mailData.EmailToIds.Add(foundEmployeeData.Supervisor.EmailAddress);
                    mailData.EmailToIds.Add(foundEmployeeData.EmailAddress);
                    mailData.EmailToIds.Add(emailHRManager.Email);
                    mailData.EmailBody = htmlTemplate;

                    var emailResult = _emailService.SendEmailAsync(mailData);

                }

            }
            return new Response
            {
                Status = "Success",
                Message = "Process review result is out"
            };
        }
    }
}
