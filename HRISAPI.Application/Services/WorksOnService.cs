using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.WorksOn;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using HRISAPI.Application.DTO.User;

namespace HRISAPI.Application.Services
{
    public class WorksOnService : IWorksOnService
    {
        private readonly IWorksOnRepository _worksOnRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDepartmentRepository _departmentRepository;
        public WorksOnService(IWorksOnRepository worksOnRepository, IProjectRepository projectRepository, IEmployeeRepository employeeRepository,IHttpContextAccessor httpContextAccessor,IDepartmentRepository departmentRepository)
        {
            _worksOnRepository = worksOnRepository;
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
        }
        public async Task<Response> AddWorksOn(DTOWorksOn inputWorksOn)
        {
            if (!await _projectRepository.AnyAsync(pro => pro.ProjectId == inputWorksOn.ProjNo))
            {
                throw new ArgumentException("Invalid Project ID");
            }
            if (!await _employeeRepository.AnyAsync(e => e.EmployeeId == inputWorksOn.EmpNo))
            {
                throw new ArgumentException("Invalid Employee ID");
            }
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);

            var foundUser = await _employeeRepository.GetFirstOrDefaultAsync(e =>e.EmployeeId== inputWorksOn.EmpNo);
            var foundProject = await _projectRepository.GetFirstOrDefaultAsync(p => p.ProjectId == inputWorksOn.ProjNo,"Department");

            if (isAdmin)
            {

            }
            else if (isDepartmentManager)
            {    
                if (foundProject.Department.MgrEmpNo != intEmployeeId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            else if (isEmployeeSupervisor)
            {
                if(foundUser.SuperVisorId != intEmployeeId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            var newWorksOn = new WorksOn
            {
                EmpNo = inputWorksOn.EmpNo,
                ProjNo = inputWorksOn.ProjNo,
                Hoursworked = inputWorksOn.Hoursworked
            };
            await _worksOnRepository.AddAsync(newWorksOn);
            await _worksOnRepository.SaveAsync();
            return new Response
            {
                Status = "Success",
                Message ="WorksOn Created successfully"
            };
        }
        public async Task<IEnumerable<DTOWorksOnDetail>> GetAllWorksOns()
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);
            bool isHrManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);

            var worksOns = await _worksOnRepository.GetAllAsync("Project,Employee,Department");

            if (isAdmin || isHrManager)
            {

            }
            else if (isDepartmentManager)
            {
                var foundDepartment = await _departmentRepository.GetFirstOrDefaultAsync(d => d.MgrEmpNo == intEmployeeId);
                worksOns = worksOns.Where(w => w.Project.DeptId == foundDepartment.DepartmentId);
            }
            else if (isEmployeeSupervisor)
            {
                worksOns = worksOns.Where(w => w.Employee.SuperVisorId == intEmployeeId);
            }
            else if (isEmployee)
            {
                worksOns = worksOns.Where(w => w.EmpNo == intEmployeeId);
            }
            var dtoWorksOn = worksOns.Select(w => new DTOWorksOnDetail
            {
                EmpName = w.Employee.EmployeeName,
                ProjName = w.Project.Name,
                DeptName = w.Project.Department != null ? w.Project.Department.Name : "No Department",
                SuperVisorName = w.Employee.Supervisor != null ? w.Employee.Supervisor.EmployeeName : "No Supervisor",
                Hoursworked = w.Hoursworked
            }).ToList();
            return dtoWorksOn;
        }
        public async Task<WorksOn> GetWorksOnById(int id)
        {
            WorksOn chosenWorksOn = await _worksOnRepository.GetFirstOrDefaultAsync(foundWorksOn => foundWorksOn.WorksOnId == id);
            return chosenWorksOn;
        }

        private async Task<WorksOn> GetWorksOnByIdBasedOnRoles(List<string> userRoles,int? employeeId,int id)
        {
            WorksOn chosenWorksOn = await _worksOnRepository.GetFirstOrDefaultAsync(foundWorksOn => foundWorksOn.WorksOnId == id,"Project,Employee");

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);

            if (isAdmin) 
            {
                
            }
            else if (isDepartmentManager)
            {
                var foundDepartment = await _departmentRepository.GetFirstOrDefaultAsync(d => d.MgrEmpNo == employeeId);
                if(chosenWorksOn.Project.DeptId != foundDepartment.DepartmentId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            else if (isEmployeeSupervisor)
            {
                if(chosenWorksOn.Employee.SuperVisorId != employeeId)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            return chosenWorksOn;
        }
        public async Task<DTOWorksOnDetail> UpdateWorksOn(DTOWorksOn worksOn, int id)
        {
            if (!await _projectRepository.AnyAsync(pro => pro.ProjectId == worksOn.ProjNo))
            {
                throw new ArgumentException("Invalid Project ID");
            }
            if (!await _employeeRepository.AnyAsync(e => e.EmployeeId == worksOn.EmpNo))
            {
                throw new ArgumentException("Invalid Employee ID");
            }
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            var foundWorksOn = await GetWorksOnByIdBasedOnRoles(userRoles,intEmployeeId,id);
            if (foundWorksOn is null)
            {
                return null;
            }
            _worksOnRepository.Update(foundWorksOn, worksOn);
            await _worksOnRepository.SaveAsync();

            var dtoWorksOn = new DTOWorksOnDetail
            {
                EmpName = foundWorksOn.Employee.EmployeeName,
                ProjName = foundWorksOn.Project.Name,
                DeptName = foundWorksOn.Project.Department != null ? foundWorksOn.Project.Department.Name : "No Department",
                SuperVisorName = foundWorksOn.Employee.Supervisor != null ? foundWorksOn.Employee.Supervisor.EmployeeName : "No Supervisor",
                Hoursworked = foundWorksOn.Hoursworked
            };

            return dtoWorksOn;
        }
        public async Task<bool> DeleteWorksOn(int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            var foundWorksOn = await GetWorksOnByIdBasedOnRoles(userRoles, intEmployeeId, id);
            if (foundWorksOn is null)
            {
                return false;
            }
            _worksOnRepository.Remove(foundWorksOn);
            await _worksOnRepository.SaveAsync();
            return true;
        }
        public async Task<byte[]> GenerateProjectReportPDF()
        {
            var projects = await _worksOnRepository.GetProjectReport();

            string htmlContent = $"<h1>Report of Projects</h1>";
            htmlContent += "<table>";
            htmlContent += "<tr>" +
                "<th>Project Name</th>"+
                "<th>Total Employees</th>" +
                "<th>Total Hours</th>" +
                "<th>Average Hours</th>" +
                "</tr>";

            foreach (var project in projects)
            {
                htmlContent += $"<tr>" +
                               $"<td>{project.ProjectName}</td>" +
                               $"<td>{project.TotalEmployees}</td>" +
                               $"<td>{project.TotalHours}</td>" +
                               $"<td>{project.AverageHours}</td>" +
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
        public async Task<IEnumerable<WorksOnProjectReport>> GetGroupedProjects()
        {
            var projects = await _worksOnRepository.GetProjectReport();
            return projects;
        }
    }
}
