using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using HRISAPI.Domain.Models;
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
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] DTODepartmentAdd department)
        {
            var inputEmployee = await _departmentService.AddDepartment(department);
            return Ok(inputEmployee);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments([FromQuery] QueryParameterDepartment? queryParameter)
        {
            var departments = await _departmentService.GetAllDepartments(queryParameter);
            return Ok(departments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            DTODepartment department = await _departmentService.GetDepartmentDetailById(id);
            return Ok(department);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDepartment([FromBody] DTOResultDepartmentAdd department, int id)
        {
            var updatedDepartment = await _departmentService.UpdateDepartment(department, id);
            return Ok(updatedDepartment);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartment(id);
            return Ok("Department is deleted");
        }
    }
}
