using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.User;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface IEmployeeService
    {
        Task<DTOEmployeeGetAll> AddEmployee(DTOEmployeeAdd employee);
        Task<IEnumerable<DTOEmployeeGetAll>> GetAllEmployees(QueryParameter.QueryParameter? queryParameter);
        Task<Employee> GetEmployeeById(int id);
        Task<DTOEmployeeGetDetail> GetEmployeeDetail(int id);
        Task<DTOUpdatedEmployee> UpdateEmployee(DTOEmployeeAdd employee, int id);
        Task <Response> AssignEmployeeToDepartment(int id);
        Task<bool> DeleteEmployee(int id);
        Task<DTODeactivateEmployee> DeactivateEmployee(int id, string deleteReasoning);
    }
}
