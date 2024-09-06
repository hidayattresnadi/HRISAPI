using HRISAPI.Application.DTO.WorkflowSequence;

namespace HRISAPI.Application.IServices
{
    public interface IWorkflowSequenceService
    {
        Task<bool> AddWorkflowSequence(WorkflowSequenceAdd workflowSequenceAdd);
    }
}
