using HRISAPI.Application.DTO.WorkflowSequence;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.Services
{
    public class WorkflowSequenceService :IWorkflowSequenceService
    {
        private readonly IWorkflowSequenceRepository _workflowSequenceRepository;
        public WorkflowSequenceService(IWorkflowSequenceRepository workflowSequenceRepository)
        {
            _workflowSequenceRepository = workflowSequenceRepository;
        }
        public async Task<bool> AddWorkflowSequence(WorkflowSequenceAdd workflowSequenceAdd)
        {
            var workflowSequence = new WorkflowSequence
            {
                RequiredRole = workflowSequenceAdd.RequiredRole,
                StepName = workflowSequenceAdd.StepName,
                StepOrder = workflowSequenceAdd.StepOrder,
                WorkflowId = workflowSequenceAdd.WorkflowId
            };
            await _workflowSequenceRepository.AddAsync(workflowSequence);
            await _workflowSequenceRepository.SaveAsync();
            return true;
        }
    }
}
