using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.Employee;
using HRISAPI.Application.DTO.User;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Employee Update(Employee foundEmployee, DTOEmployeeAdd employee);
        Employee UpdateForEmployee(Employee foundEmployee, DTOEmployeeAdd employee);
        Task<Employee> AssignEmployeeToDepartment(Employee employee, int id);
        Task<IEnumerable<Employee>> GetAllEmployeesSorted(string? includeProperties = null,QueryParameter.QueryParameter? queryParameter = null);
        Task<Employee> DeactivateEmployee(Employee employee, string deleteReasoning);
        Task<IEnumerable<EmployeeDistributionDTO>> GetEmployeesDistribution();
        Task<IEnumerable<DepartmentSallaryDTO>> GetDepartmentSallaries();
    }
}
