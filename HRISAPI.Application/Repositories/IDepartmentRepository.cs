using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Department Update(Department foundDepartment, DTOResultDepartmentAdd department);
        Task<IEnumerable<Department>> GetAllDepartmentsSorted(QueryParameterDepartment? queryParameter = null);
    }
}
