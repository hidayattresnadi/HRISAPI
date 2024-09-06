using HRISAPI.Application.DTO.WorkflowDTO;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;
        public WorkflowService(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<bool> AddWorkflow(WorkflowRequest inputWorkflow)
        {
            var newWorkflow = new Workflow
            {
                WorkflowName = inputWorkflow.WorkflowName,
                Description = inputWorkflow.Description
            };
            await _workflowRepository.AddAsync(newWorkflow);
            await _workflowRepository.SaveAsync();
            return true;
        }
    }
}
