using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.Process;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorksOnRepository _worksOnRepository;
        private readonly IProcessRepository _processRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardService(IEmployeeRepository employeeRepository, IWorksOnRepository worksOnRepository, IProcessRepository processRepository, IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _worksOnRepository = worksOnRepository;
            _processRepository = processRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DashboardDTO> GetDashboardInfo()
        {
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            IEnumerable<Process> process = Enumerable.Empty<Process>();
            process = await _processRepository.GetProcessBasedOnRole(userRoles);

            var processUsersDTO = process.Select(p => new ProcessDetailDTO
            {
                ProcessId = p.ProcessId,
                WorkflowName = p.Workflow.WorkflowName,
                Requester = p.Requester.UserName,
                RequestDate = p.RequestDate,
                Status = p.Status,
                CurrentStep = p.WorkflowSequence.StepName
            }).ToList();

            var distributionEmployee = await _employeeRepository.GetEmployeesDistribution();
            var mostActiveEmployees = await _worksOnRepository.GetMostProductiveEmployees();
            var departmentsSallaries = await _employeeRepository.GetDepartmentSallaries();
            
            var dashboardInfo = new DashboardDTO { 
                                EmployeeDistributions = distributionEmployee, 
                                MostProductiveEmployees = mostActiveEmployees,
                                DepartmentSallaries = departmentsSallaries,
                                ProcessFollowUp = processUsersDTO
            };
            return dashboardInfo;
        }
    }
}
