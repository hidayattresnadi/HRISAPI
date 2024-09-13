using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Employee;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Mail;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRISAPI.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkflowActionRepository _workflowActionRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IWorkflowSequenceRepository _workflowSequenceRepository;
        private readonly INextStepRulesRepository _nextStepRulesRepository;
        private readonly IEmailService _emailService;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IHttpContextAccessor httpContextAccessor,
                               IWorkflowActionRepository workflowActionRepository, IRequestRepository requestRepository, IProcessRepository processRepository,
                               IWorkflowSequenceRepository workflowSequenceRepository, INextStepRulesRepository nextStepRulesRepository,
                               IEmailService emailService, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILeaveRequestRepository leaveRequestRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _httpContextAccessor = httpContextAccessor;
            _workflowActionRepository = workflowActionRepository;
            _requestRepository = requestRepository;
            _processRepository = processRepository;
            _workflowSequenceRepository = workflowSequenceRepository;
            _nextStepRulesRepository = nextStepRulesRepository;
            _emailService = emailService;
            _roleManager = roleManager;
            _userManager = userManager;
            _leaveRequestRepository = leaveRequestRepository;
        }
        public async Task<DTOEmployeeGetAll> AddEmployee(DTOEmployeeAdd inputEmployee)
        {
            if (inputEmployee.DepartmentId != null)
            {
                var department = await _departmentRepository.GetFirstOrDefaultAsync(d => d.DepartmentId == inputEmployee.DepartmentId);
                if (department == null)
                {
                    throw new BadRequestException("DepartmentId is invalid");
                }
            }
            var today = DateTime.UtcNow;
            var newEmployee = new Employee
            {
                DepartmentId = inputEmployee.DepartmentId,
                EmployeeName = inputEmployee.EmployeeName,
                SSN = inputEmployee.SSN,
                Address = inputEmployee.Address,
                Sallary = inputEmployee.Sallary,
                Sex = inputEmployee.Sex,
                BirthDate = inputEmployee.BirthDate,
                EmploymentType = inputEmployee.EmploymentType,
                Level = inputEmployee.Level,
                PhoneNumber = inputEmployee.PhoneNumber,
                EmailAddress = inputEmployee.EmailAddress,
                JobPosition = inputEmployee.JobPosition,
                Status = "Active",
                LastUpdatedDate = today,
                SuperVisorId = inputEmployee.SuperVisorId,
                Dependents = inputEmployee.Dependents.Select(d => new Dependent
                {
                    Name = d.Name,
                    Relations = d.Relations,
                    BirthDate = d.BirthDate,
                    Sex = d.Sex,
                }).ToList()
            };
            await _employeeRepository.AddAsync(newEmployee);
            await _employeeRepository.SaveAsync();
            var employeeDto = new DTOEmployeeGetAll
            {
                EmployeeName = newEmployee.EmployeeName,
                EmploymentType = newEmployee.EmploymentType,
                JobPosition = newEmployee.JobPosition,
                Level = newEmployee.Level,
                LastUpdatedDate = newEmployee.LastUpdatedDate,
                Department = newEmployee.Department.Name,
            };
            return employeeDto;
        }
        public async Task<IEnumerable<DTOEmployeeGetAll>> GetAllEmployees(QueryParameter.QueryParameter? queryParameter)
        {
            var employees = await _employeeRepository.GetAllEmployeesSorted("Department",queryParameter);
            var employeeDtos = employees.Select(employee => new DTOEmployeeGetAll
            {
                EmployeeName = employee.EmployeeName,
                EmploymentType = employee.EmploymentType,
                JobPosition = employee.JobPosition,
                Level = employee.Level,
                LastUpdatedDate = employee.LastUpdatedDate,
                Department = employee.Department.Name,
            }).ToList();
            return employeeDtos;
        }
        public async Task<Employee> GetEmployeeById(int id) 
        {
            Employee chosenEmployee = await _employeeRepository.GetFirstOrDefaultAsync(foundEmployee => foundEmployee.EmployeeId == id);
            if (chosenEmployee == null)
            {
                throw new NotFoundException("Employee is not found");
            }
            return chosenEmployee;
        }
        public async Task<DTOEmployeeGetDetail> GetEmployeeDetail(int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);
            Employee chosenEmployee = await _employeeRepository.GetFirstOrDefaultAsync((foundEmployee => foundEmployee.EmployeeId == id),"Supervisor,Department");
            if (chosenEmployee == null)
            {
                throw new NotFoundException("Employee is not found");
            }
            DTOEmployeeGetDetail employeeDetailDTO = null;
            if (isAdmin || isHRManager)
            {
                employeeDetailDTO = new DTOEmployeeGetDetail
                {
                    EmployeeName = chosenEmployee.EmployeeName,
                    Address = chosenEmployee.Address,
                    PhoneNumber = chosenEmployee.PhoneNumber,
                    EmailAddress = chosenEmployee.EmailAddress,
                    JobPosition = chosenEmployee.JobPosition,
                    SuperVisorName = chosenEmployee.Supervisor != null ? chosenEmployee.Supervisor.EmployeeName : "No Supervisor",
                    EmploymentType = chosenEmployee.EmploymentType,
                    Salary = chosenEmployee.Sallary,
                    SSN = chosenEmployee.SSN
                };

            }
            else if (isDepartmentManager)
            {
                if (chosenEmployee.Department.MgrEmpNo != intEmployeeId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
                employeeDetailDTO = new DTOEmployeeGetDetail
                {
                    EmployeeName = chosenEmployee.EmployeeName,
                    Address = chosenEmployee.Address,
                    PhoneNumber = chosenEmployee.PhoneNumber,
                    EmailAddress = chosenEmployee.EmailAddress,
                    JobPosition = chosenEmployee.JobPosition,
                    SuperVisorName = chosenEmployee.Supervisor != null ? chosenEmployee.Supervisor.EmployeeName : "No Supervisor",
                    EmploymentType = chosenEmployee.EmploymentType,
                    Salary = chosenEmployee.Sallary
                };
            }
            else if (isEmployeeSupervisor)
            {
                if (chosenEmployee.SuperVisorId != intEmployeeId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
                employeeDetailDTO = new DTOEmployeeGetDetail
                {
                    EmployeeName = chosenEmployee.EmployeeName,
                    Address = chosenEmployee.Address,
                    PhoneNumber = chosenEmployee.PhoneNumber,
                    EmailAddress = chosenEmployee.EmailAddress,
                    JobPosition = chosenEmployee.JobPosition,
                    SuperVisorName = chosenEmployee.Supervisor != null ? chosenEmployee.Supervisor.EmployeeName : "No Supervisor",
                    EmploymentType = chosenEmployee.EmploymentType,
                    Salary = chosenEmployee.Sallary
                };
            }
            else if (isEmployee)
            {
                if (intEmployeeId != id)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
                employeeDetailDTO = new DTOEmployeeGetDetail
                {
                    EmployeeName = chosenEmployee.EmployeeName,
                    Address = chosenEmployee.Address,
                    PhoneNumber = chosenEmployee.PhoneNumber,
                    EmailAddress = chosenEmployee.EmailAddress,
                    JobPosition = chosenEmployee.JobPosition,
                    SuperVisorName = chosenEmployee.Supervisor != null ? chosenEmployee.Supervisor.EmployeeName : "No Supervisor",
                    EmploymentType = chosenEmployee.EmploymentType,
                    Salary = null
                };
            }
            return employeeDetailDTO;
        }
        public async Task<DTOUpdatedEmployee> UpdateEmployee(DTOEmployeeAdd employee, int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);

            DTOUpdatedEmployee employeeUpdatedDTO = null;

            var foundEmployee = await GetEmployeeById(id);
            if (isAdmin || isHRManager)
            {
                var updatedEmployee = _employeeRepository.Update(foundEmployee, employee);
                await _employeeRepository.SaveAsync();
                employeeUpdatedDTO = new DTOUpdatedEmployee
                {
                    DepartmentId = updatedEmployee.DepartmentId,
                    EmployeeName = updatedEmployee.EmployeeName,
                    SSN = updatedEmployee.SSN,
                    Address = updatedEmployee.Address,
                    Sallary = updatedEmployee.Sallary,
                    Sex = updatedEmployee.Sex,
                    BirthDate = updatedEmployee.BirthDate,
                    EmploymentType = updatedEmployee.EmploymentType,
                    Level = updatedEmployee.Level,
                    PhoneNumber = updatedEmployee.PhoneNumber,
                    EmailAddress = updatedEmployee.EmailAddress,
                    JobPosition = updatedEmployee.JobPosition,
                    LastUpdatedDate = foundEmployee.LastUpdatedDate,
                    SuperVisorId = foundEmployee.SuperVisorId
                };
            }
            else if (isEmployee)
            {
                if(intEmployeeId != id)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
                var updatedEmployee = _employeeRepository.UpdateForEmployee(foundEmployee, employee);
                await _employeeRepository.SaveAsync();
                employeeUpdatedDTO = new DTOUpdatedEmployee
                {
                    DepartmentId = updatedEmployee.DepartmentId,
                    EmployeeName = updatedEmployee.EmployeeName,
                    Address = updatedEmployee.Address,
                    Sex = updatedEmployee.Sex,
                    BirthDate = updatedEmployee.BirthDate,
                    EmploymentType = updatedEmployee.EmploymentType,
                    Level = updatedEmployee.Level,
                    PhoneNumber = updatedEmployee.PhoneNumber,
                    EmailAddress = updatedEmployee.EmailAddress,
                    JobPosition = updatedEmployee.JobPosition,
                    LastUpdatedDate = foundEmployee.LastUpdatedDate,
                    SuperVisorId = foundEmployee.SuperVisorId
                };

            }
            return employeeUpdatedDTO;
        }
        public async Task<bool> DeleteEmployee(int id)
        {
            var foundEmployee = await GetEmployeeById(id);
            _employeeRepository.Remove(foundEmployee);
            await _employeeRepository.SaveAsync();
            return true;
        }

        public async Task<DTODeactivateEmployee> DeactivateEmployee(int id, string deleteReasoning)
        {
            var foundEmployee = await GetEmployeeById(id);
            var deactivateEmployee = await _employeeRepository.DeactivateEmployee(foundEmployee, deleteReasoning);
            var mapDeactivateEmployee = new DTODeactivateEmployee
            {
                DeleteReasoning = deactivateEmployee.DeactivationReasoning,
                Status = deactivateEmployee.Status,
                LastUpdated = deactivateEmployee.LastUpdatedDate
            };
            await _employeeRepository.SaveAsync();
            return mapDeactivateEmployee;
        }
        public async Task<Response> AssignEmployeeToDepartment(int id)
        {
            var employee = await GetEmployeeById(id);
            await _employeeRepository.AssignEmployeeToDepartment(employee, id);
            await _employeeRepository.SaveAsync();
            return new Response
            {
                Message = "Assigning Employee to department success",
                Status = "Success"
            };
        }

        public async Task<Response> AddRequestAddingLeave(EmployeeDTOLeaveRequest request, int workflowId)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var newRequest = new Request
            {
                ProcessName = "Leave Adding Request",
                Description = "Process for handling leave adding request",
                StartDate = DateTime.UtcNow
            };
            await _requestRepository.AddAsync(newRequest);
            await _requestRepository.SaveAsync();

            var currentStepId = await _workflowSequenceRepository.GetFirstOrDefaultAsync(wfs => wfs.WorkflowId == workflowId);
            var nextStepId = await _nextStepRulesRepository.GetFirstOrDefaultAsync(n => n.CurrentStepId == currentStepId.StepId);

            var newProcess = new Process
            {
                RequesterId = userId,
                WorkflowId = workflowId,
                RequestType = "Adding Leave",
                Status = "Pending",
                RequestDate = DateTime.UtcNow,
                RequestId = newRequest.RequestId,
                CurrentStepId = nextStepId.NextStepId
            };

            await _processRepository.AddAsync(newProcess);
            await _processRepository.SaveAsync();

            var newLeaveRequest = new LeaveRequest
            {
                ProcessId = newProcess.ProcessId,
                EmployeeID = request.EmployeeID,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                LeaveType = request.LeaveType,
                Reason = request.Reason
            };

            await _leaveRequestRepository.AddAsync(newLeaveRequest);
            await _leaveRequestRepository.SaveAsync();

            var newWorkflowAction = new WorkflowAction
            {
                ProcessId = newProcess.ProcessId,
                StepId = nextStepId.CurrentStepId,
                ActorId = userId,
                Action = "Request submitted",
                ActionDate = DateTime.UtcNow,
                Comments = $"{request.LeaveType}"
            };
            await _workflowActionRepository.AddAsync(newWorkflowAction);
            await _workflowActionRepository.SaveAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Response
                {
                    Status = "Error",
                    Message = "User is not found"
                };
            }
            var employeeId =user.EmployeeId;
            var superVisorData = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == employeeId,"Supervisor");

            var htmlTemplate = System.IO.File.ReadAllText(@"./Templates/EmailTemplate/AddingLeaveRequest.html");
            htmlTemplate = htmlTemplate.Replace("{{EmployeeId}}", request.EmployeeID.ToString());
            htmlTemplate = htmlTemplate.Replace("{{StartDate}}", request.StartDate.ToString("d MMMM yyyy", new CultureInfo("id-ID")));
            htmlTemplate = htmlTemplate.Replace("{{EndDate}}", request.EndDate.ToString("d MMMM yyyy", new CultureInfo("id-ID")));
            htmlTemplate = htmlTemplate.Replace("{{Name}}", superVisorData.Supervisor.EmployeeName);
            htmlTemplate = htmlTemplate.Replace("{{Leave Type}}", request.LeaveType);
            htmlTemplate = htmlTemplate.Replace("{{Reason}}", request.Reason);

            var mailData = new MailData
            {
                EmailToName = superVisorData.Supervisor.EmployeeName,
                EmailSubject = "Leave request to add",
            };

            mailData.EmailToIds.Add(superVisorData.Supervisor.EmailAddress);
            mailData.EmailToIds.Add(user.Email);
            mailData.EmailBody = htmlTemplate;

            var emailResult = _emailService.SendEmailAsync(mailData);
            return new Response
            {
                Status = "Success",
                Message = "Adding leave request success"
            };
        }
        public async Task<byte[]> GenerateEmployeeReportByDepartmentPDF(int departmentId)
        {
            var employees = await _employeeRepository.GetAllAsync(e => e.DepartmentId == departmentId);
            var chosenDepartment = await _departmentRepository.GetFirstOrDefaultAsync(d => d.DepartmentId == departmentId);
            string departmentName = $"{chosenDepartment.Name}";

            var document = new PdfDocument();
            var config = new PdfGenerateConfig
            {
                PageOrientation = PageOrientation.Landscape,
                PageSize = PageSize.A4
            };

            int maxItemsPerPage = 20;
            int pageCount = (int)Math.Ceiling(employees.Count() / (double)maxItemsPerPage);

            string cssStr = File.ReadAllText(@"./Templates/PDFReportTemplate/style2.css");
            CssData css = PdfGenerator.ParseStyleSheet(cssStr);
            for (int page = 0; page < pageCount; page++)
            {
                // Mengambil data untuk halaman saat ini
                var employeesPage = employees.Skip(page * maxItemsPerPage).Take(maxItemsPerPage).ToList();

                // Membuat konten HTML untuk halaman
                string htmlContent = $"<h1>Report of Employees Information for Department: {departmentName}</h1>";
                htmlContent += $"<h3>Page {page + 1} of {pageCount}</h3>"; // Menampilkan nomor halaman
                htmlContent += "<table>";
                htmlContent += "<tr>" +
                    "<th>Employee Name</th>" +
                    "<th>Address</th>" +
                    "<th>Salary</th>" +
                    "<th>Sex</th>" +
                    "<th>Birth Date</th>" +
                    "<th>Employment Type</th>" +
                    "<th>Phone Number</th>" +
                    "<th>Email Address</th>" +
                    "<th> Job Position</th>" +
                    "</tr>";

                foreach (var employee in employeesPage)
                {
                    htmlContent += $"<tr>" +
                                   $"<td>{employee.EmployeeName}</td>" +
                                   $"<td>{employee.Address}</td>" +
                                   $"<td>{employee.Sallary}</td>" +
                                   $"<td>{employee.Sex}</td>" +
                                   $"<td>{employee.BirthDate:yyyy-MM-dd}</td>" +
                                   $"<td>{employee.EmploymentType}</td>" +
                                   $"<td>{employee.PhoneNumber}</td>" +
                                   $"<td>{employee.EmailAddress}</td>" +
                                   $"<td>{employee.JobPosition}</td>" +
                                   $"</tr>";
                }

                htmlContent += "</table>";

                // Menambahkan halaman ke dokumen PDF
                PdfGenerator.AddPdfPages(document, htmlContent, config, css);
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }

        public async Task<IEnumerable<EmployeeDetailPDF>> GetEmployeeDataPraPDF(int departmentId)
        {
            var employees = await _employeeRepository.GetAllAsync(e => e.DepartmentId == departmentId);
            var employeesDTO = employees.Select(e => new EmployeeDetailPDF
            {
                EmployeeName = e.EmployeeName,
                Address = e.Address,
                Sallary = e.Sallary,
                Sex = e.Sex,
                BirthDate = e.BirthDate,
                EmploymentType = e.EmploymentType,
                PhoneNumber = e.PhoneNumber,
                EmailAddress = e.EmailAddress,
                JobPosition = e.JobPosition
            }).ToList();

            return employeesDTO;
        }
    }
}
