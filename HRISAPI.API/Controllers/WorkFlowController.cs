using HRISAPI.Application.DTO.WorkflowDTO;
using HRISAPI.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;
        public WorkflowController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }
        [HttpPost]
        public async Task<IActionResult> AddWorkflow([FromBody]WorkflowRequest workflowRequest)
        {
            string message = "success created flow";
            var isAddWorkflow = await _workflowService.AddWorkflow(workflowRequest);
            if (isAddWorkflow != true)
            {
                return BadRequest("Sorry, the process failed");
            }
            return Ok(message);
        }
    }
}
