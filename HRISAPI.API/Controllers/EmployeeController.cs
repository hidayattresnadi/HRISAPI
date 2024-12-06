using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Employee;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Services;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IWebHostEnvironment _environment;
        public EmployeeController(IEmployeeService employeeService, ILeaveRequestService leaveRequestService, IWebHostEnvironment webHostEnvironment)
        {
            _employeeService = employeeService;
            _leaveRequestService = leaveRequestService;
            _environment = webHostEnvironment;

        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_HR_Manager)]
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] DTOEmployeeAdd employee)
        {
            var inputEmployee = await _employeeService.AddEmployee(employee);
            return Ok(inputEmployee);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] QueryParameter? queryParameter)
        {
            var employees = await _employeeService.GetAllEmployees(queryParameter);
            return Ok(employees);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            DTOEmployeeGetDetail employee = await _employeeService.GetEmployeeDetail(id);
            return Ok(employee);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_HR_Manager+","+Roles.Role_Employee)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee([FromBody] DTOEmployeeAdd employee, int id)
        {
            var updatedEmployee = await _employeeService.UpdateEmployee(employee, id);
            return Ok(updatedEmployee);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_HR_Manager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteEmployee(id);
            return Ok("Employee is deleted");
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_HR_Manager)]
        [HttpPatch("Deactivate_Employee/{id}")]
        public async Task<IActionResult> DeactivateEmployee(int id,[FromBody] string deleteReasoning)
        {
            var deactivateEmployee = await _employeeService.DeactivateEmployee(id,deleteReasoning);
            return Ok(deactivateEmployee);
        }
        [Authorize(Roles = Roles.Role_HR_Manager)]
        [HttpPatch("Assigning_Employee/{id}")]
        public async Task<IActionResult> AssigningEmployee(int id)
        {
            var response = await _employeeService.AssignEmployeeToDepartment(id);
            return Ok(response);
        }
        [Authorize(Roles = Roles.Role_Employee)]
        [HttpPost("leave_request/{id}")]
        public async Task<IActionResult> AddRequestAddingBook([FromBody]EmployeeDTOLeaveRequest request, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _employeeService.AddRequestAddingLeave(request, id);
            if (result.Status == "Error")
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpGet("generate_report/{departmentId}")]
        public async Task<IActionResult> EmployeeReport(int departmentId)
        {
            var Filename = "EmployeeReport.pdf";

            var file = await _employeeService.GenerateEmployeeReportByDepartmentPDF(departmentId);

            return File(file, "application/pdf", Filename);
        }
        [HttpGet("pra_PDF/{departmentId}")]
        public async Task<IActionResult> EmployeeReportJSON(int departmentId)
        {
            var employees = await _employeeService.GetEmployeeDataPraPDF(departmentId);
            return Ok(employees);
        }

        [Authorize]
        [HttpGet("leave_request")]
        public async Task<IActionResult> GetLeavesRequestLists([FromQuery] QueryParameterLeaveRequest request)
        {
            var leavesType = await _leaveRequestService.GetLeaveRequestsLists(request);
            return Ok(leavesType);
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(IFormFile formFile)
        {
            try
            {
                //max size upload 5MB
                long maxFileSize = 5 * 1024 * 1024;
                string[] AllowedFileTypes =
                {
                    "application/pdf", // PDF
                    "image/jpeg", // JPG/JPEG
                    "image/jpg", // JPG
                };


                if (formFile == null || formFile.Length == 0)
                {
                    return BadRequest("File is empty");
                }

                if (formFile.Length > maxFileSize)
                {
                    return BadRequest("File exceeds 2 MB limit");
                }

                if (!AllowedFileTypes.Contains(formFile.ContentType))
                {
                    return BadRequest("Only pdf and word documents are allowed");
                }
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to directory

                using (var fileStream = new FileStream(filePath, FileMode.Create))

                {

                    await formFile.CopyToAsync(fileStream);

                }

                return Ok($"{uniqueFileName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            // Tentukan lokasi folder tempat file disimpan
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            // Periksa apakah file ada
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); 
            }

            // Mengembalikan file untuk diunduh
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}
