using HRISAPI.Application.DTO.NextStepRules;
using HRISAPI.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NextStepRulesController : ControllerBase
    {
        private readonly INextStepRulesService _nextStepRulesService;
        public NextStepRulesController(INextStepRulesService nextStepRulesService)
        {
            _nextStepRulesService = nextStepRulesService;
        }
        [HttpPost]
        public async Task<IActionResult> AddNextStepRules([FromBody] NextStepRulesAdd request)
        {
            string message = "success created next step rules";
            var isNextStepRules = await _nextStepRulesService.AddNextStepRules(request);
            if (isNextStepRules != true)
            {
                return BadRequest("Sorry, the process failed");
            }
            return Ok(message);
        }
    }
}
