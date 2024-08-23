using HRISAPI.Application.DTO;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] DTOEmployeeAdd employee)
        {
            var inputEmployee = await _employeeService.AddEmployee(employee);
            return Ok(inputEmployee);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] QueryParameter? queryParameter)
        {
            var employees = await _employeeService.GetAllEmployees(queryParameter);
            return Ok(employees);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            DTOEmployeeGetDetail employee = await _employeeService.GetEmployeeDetail(id);
            return Ok(employee);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee([FromBody] DTOEmployeeAdd employee, int id)
        {
            var updatedEmployee = await _employeeService.UpdateEmployee(employee, id);
            return Ok(updatedEmployee);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteEmployee(id);
            return Ok("Employee is deleted");
        }

        [HttpPatch("Deactivate_Employee/{id}")]
        public async Task<IActionResult> DeactivateEmployee(int id,[FromBody] string deleteReasoning)
        {
            var deactivateEmployee = await _employeeService.DeactivateEmployee(id,deleteReasoning);
            return Ok(deactivateEmployee);
        }
    }
}
