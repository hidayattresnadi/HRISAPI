using HRISAPI.Application.DTO;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
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
            Employee chosenEmployee = await _employeeRepository.GetFirstOrDefaultAsync((foundEmployee => foundEmployee.EmployeeId == id),"Supervisor");
            if (chosenEmployee == null)
            {
                throw new NotFoundException("Employee is not found");
            }
            var employeeDetailDTO = new DTOEmployeeGetDetail
            {
                EmployeeName = chosenEmployee.EmployeeName,
                Address = chosenEmployee.Address,
                PhoneNumber = chosenEmployee.PhoneNumber,
                EmailAddress = chosenEmployee.EmailAddress,
                JobPosition = chosenEmployee.JobPosition,
                SuperVisorName = chosenEmployee.Supervisor != null ? chosenEmployee.Supervisor.EmployeeName : "No Supervisor",
                EmploymentType= chosenEmployee.EmploymentType,

            };
            return employeeDetailDTO;
        }
        public async Task<DTOUpdatedEmployee> UpdateEmployee(DTOEmployeeAdd employee, int id)
        {
            var foundEmployee = await GetEmployeeById(id);
            var updatedEmployee = _employeeRepository.Update(foundEmployee, employee);
            await _employeeRepository.SaveAsync();
            var updatedEmployeeDTO = new DTOUpdatedEmployee
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
            return updatedEmployeeDTO;
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
    }
}
