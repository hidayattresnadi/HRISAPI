using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRISAPI.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _httpContextAccessor = httpContextAccessor;
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
    }
}
