using HRISAPI.Application.DTO.Process;
using HRISAPI.Application.DTO.User;

namespace LibrarySystem.Application.IServices
{
    public interface IProcessService
    {
        Task<Response> ReviewRequest(int processId, ProcessDTOApproved requestApproval);
    }
}
