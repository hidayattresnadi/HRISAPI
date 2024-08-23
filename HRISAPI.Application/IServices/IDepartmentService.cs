using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.DTO.Department.HRISAPI.Application.DTO.Department;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.IServices
{
    public interface IDepartmentService
    {
        Task<DTOResultDepartmentAdd> AddDepartment(DTODepartmentAdd department);
        Task<IEnumerable<DTODepartmentLocation>> GetAllDepartments(QueryParameterDepartment? queryParameter);
        Task<Department> GetDepartmentById(int id);
        Task<DTODepartment> GetDepartmentDetailById(int id);
        Task<DTOResultDepartmentAdd> UpdateDepartment(DTOResultDepartmentAdd department, int id);
        Task<bool> DeleteDepartment(int id);
    }
}
