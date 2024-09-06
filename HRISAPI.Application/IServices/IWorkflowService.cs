using HRISAPI.Application.DTO.WorkflowDTO;

namespace HRISAPI.Application.IServices
{
    public interface IWorkflowService
    {
        Task<bool> AddWorkflow(WorkflowRequest inputWorkflow);
    }
}
