using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.WorksOn;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorksOnController : ControllerBase
    {
        private readonly IWorksOnService _worksOnService;
        public WorksOnController(IWorksOnService worksOnService)
        {
            _worksOnService = worksOnService;
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager + "," + Roles.Role_Employee_Supervisor)]
        [HttpPost]
        public async Task<IActionResult> AddWorksOn([FromBody] DTOWorksOn worksOn)
        {
            try
            {
                var inputWorksOn = await _worksOnService.AddWorksOn(worksOn);
                return Ok(inputWorksOn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllWorksOns()
        {
            var worksOns = await _worksOnService.GetAllWorksOns();
            return Ok(worksOns);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorksOnById(int id)
        {
            WorksOn worksOn = await _worksOnService.GetWorksOnById(id);
            if (worksOn == null)
            {
                return NotFound();
            }
            return Ok(worksOn);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager + "," + Roles.Role_Employee_Supervisor)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditWorksOn([FromBody] DTOWorksOn worksOn, int id)
        {
            try
            {
                var updatedWorksOn = await _worksOnService.UpdateWorksOn(worksOn, id);
                if (updatedWorksOn == null)
                {
                    return NotFound();
                }
                return Ok(updatedWorksOn);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager + "," + Roles.Role_Employee_Supervisor)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorksOn(int id)
        {
            bool isDeletedWorksOn = await _worksOnService.DeleteWorksOn(id);
            if (isDeletedWorksOn == false)
            {
                return NotFound();
            }
            return Ok("WorksOn is deleted");
        }
    }
}
