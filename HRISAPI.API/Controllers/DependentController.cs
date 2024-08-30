using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.DTO;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DependentController : ControllerBase
    {
        private readonly IDependentService _dependentService;
        public DependentController(IDependentService dependentService)
        {
            _dependentService = dependentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDependent([FromBody] DependentDTO dependent)
        {
            var inputDependent = await _dependentService.AddDependent(dependent);
            return Ok(inputDependent);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllDependents()
        {
            var dependents = await _dependentService.GetAllDependents();
            return Ok(dependents);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDependentById(int id)
        {
            var dependent = await _dependentService.GetDependentDetailById(id);
            return Ok(dependent);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDependent([FromBody] DependentDTO dependent, int id)
        {
            var updatedDependent = await _dependentService.UpdateDependent(dependent, id);
            return Ok(updatedDependent);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDependent(int id)
        {
            await _dependentService.DeleteDependent(id);
            return Ok("Dependent is deleted");
        }
    }
}
