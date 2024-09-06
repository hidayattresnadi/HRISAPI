using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Employee
{
    public class EmployeeDTOLeaveRequest
    {
        public int EmployeeID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
