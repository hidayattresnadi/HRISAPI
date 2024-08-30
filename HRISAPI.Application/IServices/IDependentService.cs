using HRISAPI.Application.DTO.Dependent;
using HRISAPI.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface IDependentService
    {
        Task<bool> DeleteDependent(int id);
        Task<DependentDTODetail> UpdateDependent(DependentDTO dependent, int id);
        Task<IEnumerable<DependentDTODetail>> GetAllDependents();
        Task<DependentDTODetail> GetDependentDetailById(int id);
        Task<DependentDTO> AddDependent(DependentDTO inputDependent);
    }
}
