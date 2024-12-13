using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Application.DTO;
using System.Linq.Expressions;
using System.Security.Claims;
using HRISAPI.Application.QueryParameter;
using Microsoft.AspNetCore.Http;
using HRISAPI.Domain.Models;
using LinqKit;
using System.Drawing;
using Microsoft.AspNetCore.Identity;

namespace HRISAPI.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IWorkflowActionRepository _workflowActionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IProcessRepository processRepository, IWorkflowActionRepository workflowActionRepository, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _processRepository = processRepository;
            _workflowActionRepository = workflowActionRepository;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<byte[]> GenerateLeaveRequestsPDF(LeaveRequestDTOFiltered request)
        {
            var leaveRequests = await _leaveRequestRepository.GetGroupedLeaveRequests(request);
            string Name = $"{request.StartDate} - {request.EndDate}";

            string htmlContent = $"<h1>Report of Leave Requests in period: {Name}</h1>";
            htmlContent += "<table>";
            htmlContent += "<tr>" +
                "<th>Leave Type</th>" +
                "<th>Total Leaves</th>" +
                "</tr>";

            foreach (var leaveRequest in leaveRequests)
            {
                htmlContent += $"<tr>" +
                               $"<td>{leaveRequest.LeaveType}</td>" +
                               $"<td>{leaveRequest.TotalLeaves}</td>" +
                               $"</tr>";
            }

            htmlContent += "</table>";

            var document = new PdfDocument();
            var config = new PdfGenerateConfig
            {
                PageOrientation = PageOrientation.Landscape,
                PageSize = PageSize.A4
            };

            string cssStr = File.ReadAllText(@"./Templates/PDFReportTemplate/style.css");
            CssData css = PdfGenerator.ParseStyleSheet(cssStr);
            PdfGenerator.AddPdfPages(document, htmlContent, config, css);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }
        public async Task<IEnumerable<LeaveRequestGroupDTO>> GetLeavesType(LeaveRequestDTOFiltered request)
        {
            var leaveRequests = await _leaveRequestRepository.GetGroupedLeaveRequests(request);
            return leaveRequests;
        }

        public async Task<object> GetLeaveRequestDetail(int id)
        {
            var getLeaveRequest = await _leaveRequestRepository.GetFirstOrDefaultAsync(br => br.ProcessId == id, "Employee");
            if (getLeaveRequest == null)
            {
                return null;
            }

            var chosenProcess = await _processRepository.GetFirstOrDefaultAsync(p => p.ProcessId == id, "Requester,WorkflowSequence");
            var requestHistory = await _workflowActionRepository.GetAllAsync(wfa => wfa.ProcessId == id, "Actor", rh => rh.ActionId);
            var role = await _roleManager.FindByIdAsync(chosenProcess.WorkflowSequence.RequiredRole);

            var requestHistoryDto = requestHistory.Select(x => new
            {
                ActionDate = x.ActionDate,
                ActionBy = x.Actor?.UserName,
                Action = x.Action,
                Comments = x.Comments,
            }).ToList();

            var result = new
            {
                Requester = chosenProcess.Requester.UserName,
                RequestDate = chosenProcess.RequestDate,
                ProcessId = chosenProcess.ProcessId,
                CurrentStatus = chosenProcess.Status,
                EmployeeName = getLeaveRequest.Employee.EmployeeName,
                StartDate = getLeaveRequest.StartDate,
                EndDate = getLeaveRequest.EndDate,
                TotalDays = ((getLeaveRequest.EndDate).Day - (getLeaveRequest.StartDate).Day),
                LeaveType = getLeaveRequest.LeaveType,
                Reason = getLeaveRequest.Reason,
                RequestHistory = requestHistoryDto,
                RequiredRole = role ?? null,
            };
            return result;
        }
        public async Task<object> GetLeaveRequestsLists(QueryParameterLeaveRequest request)
        {
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
              .Where(c => c.Type == ClaimTypes.Role)
              .Select(c => c.Value)
              .ToList();

            var userRoleIds = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == "RoleId")
                .Select(c => c.Value)
                .ToList();

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isEmployeeSuperVisor = userRoles.Contains(Roles.Role_Employee_Supervisor);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);

            var userData = await _userManager.FindByIdAsync(userId);

            Expression<Func<LeaveRequest, bool>> expression = lr => true;

            if (isHRManager)
            {
                expression = expression.And(lr => userRoleIds.Contains(lr.Process.WorkflowSequence.RequiredRole));
            }
            else if (isEmployeeSuperVisor)
            {
                expression = expression.And(lr => userRoleIds.Contains(lr.Process.WorkflowSequence.RequiredRole) && lr.Process.Requester.Employee.SuperVisorId == userData.EmployeeId  );
            }
            else if (isEmployee)
            {
                expression = expression.And(lr => lr.Process.RequesterId == userId);
            }

            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                expression = expression.And(lr =>
                    lr.Employee.EmployeeName.ToLower().Contains(request.KeyWord.ToLower()) ||
                    lr.LeaveType.ToLower().Contains(request.KeyWord.ToLower()));
            }



            var (leaveRequests, totalCount) = await _leaveRequestRepository.GetAllLeaveRequestList(request, expression);


            var leaveRequestDTOs = leaveRequests.Select(leaveRequest => new
            {
                EmployeeName = leaveRequest.Process.Requester.Employee.EmployeeName ?? "N/A",  // Jika Employee null, beri nilai default "N/A"
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = leaveRequest.StartDate != null && leaveRequest.EndDate != null
        ? (leaveRequest.EndDate.Day - leaveRequest.StartDate.Day)
        : 0,  // Pastikan StartDate dan EndDate tidak null, jika null TotalDays = 0
                LeaveType = leaveRequest.LeaveType ?? "Unknown",  // Jika LeaveType null, beri nilai default "Unknown"
                Reason = leaveRequest.Reason ?? "No reason provided",  // Jika Reason null, beri nilai default "No reason provided"
                ProcessId = leaveRequest.ProcessId,
                RequestDate = leaveRequest.Process?.RequestDate ?? DateTime.MinValue,  // Jika Process null, beri nilai default DateTime.MinValue
                Status = leaveRequest.Process?.Status ?? "Pending",  // Jika Process null, beri nilai default "Pending"
                FileName = string.IsNullOrWhiteSpace(leaveRequest.FileName) ? "No file name provided" : leaveRequest.FileName,
        });



            var totalPages = (int)Math.Ceiling((decimal)(totalCount) / request.PageSize);


            var result = new
            {
                Data = leaveRequestDTOs,
                TotalPages = totalPages,
            };

            return result;

        }
    }
}
