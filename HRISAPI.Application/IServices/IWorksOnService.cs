using HRISAPI.Application.DTO.WorksOn;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.IServices
{
    public interface IWorksOnService
    {
        Task<WorksOn> AddWorksOn(DTOWorksOn worksOn);
        Task<IEnumerable<DTOWorksOnDetail>> GetAllWorksOns();
        Task<WorksOn> GetWorksOnById(int id);
        Task<DTOWorksOnDetail> UpdateWorksOn(DTOWorksOn workson, int id);
        Task<bool> DeleteWorksOn(int id);
    }
}
