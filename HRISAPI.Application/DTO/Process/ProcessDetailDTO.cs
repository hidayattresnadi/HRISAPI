using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Process
{
    public class ProcessDetailDTO
    {
        public int ProcessId { get; set; }
        public string WorkflowName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string CurrentStep { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public int? TotalDays { get; set; }
    }
}
