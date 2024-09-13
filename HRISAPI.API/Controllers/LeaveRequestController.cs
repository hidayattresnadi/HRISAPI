using HRISAPI.Application.DTO.Employee;
using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;
        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }
        [HttpGet("generate_report")]
        public async Task<IActionResult> LeaveRequestReport([FromQuery] LeaveRequestDTOFiltered request)
        {
            var Filename = "LeaveRequestReport.pdf";

            var file = await _leaveRequestService.GenerateLeaveRequestsPDF(request);

            return File(file, "application/pdf", Filename);
        }
        [HttpGet("leaves_type")]
        public async Task<IActionResult> GetLeavesType([FromQuery] LeaveRequestDTOFiltered request)
        {
            var leavesType = await _leaveRequestService.GetLeavesType(request);
            return Ok(leavesType);
        }
    }
}
