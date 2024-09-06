using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DTODepartmentAdd department)
        {
            var inputDepartment = await _departmentService.AddDepartment(department);
            return Ok(inputDepartment);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments([FromQuery] QueryParameterDepartment? queryParameter)
        {
            var departments = await _departmentService.GetAllDepartments(queryParameter);
            return Ok(departments);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            DTODepartment department = await _departmentService.GetDepartmentDetailById(id);
            return Ok(department);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDepartment([FromBody] DTOResultDepartmentAdd department, int id)
        {
            var updatedDepartment = await _departmentService.UpdateDepartment(department, id);
            return Ok(updatedDepartment);
        }
        [Authorize(Roles = Roles.Role_Administrator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartment(id);
            return Ok("Department is deleted");
        }
    }
}
