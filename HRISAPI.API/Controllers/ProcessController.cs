using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Process;
using HRISAPI.Application.IServices;
using LibrarySystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessService _processService;
        private readonly IEmailService _emailService;
        public ProcessController(IProcessService processService, IEmailService emailService)
        {
            _processService = processService;
            _emailService = emailService;
        }
        [Authorize(Roles = Roles.Role_Employee_Supervisor + "," + Roles.Role_HR_Manager)]
        [HttpPost("{id}")]
        public async Task<IActionResult> ReviewRequestBookAdding([FromBody] ProcessDTOApproved requestApproval, int id)
        {
            var result = await _processService.ReviewRequest(id,requestApproval);

            if (result.Status == "Error")

                return BadRequest(result.Message);

            return Ok(result);
        }

    }
}
