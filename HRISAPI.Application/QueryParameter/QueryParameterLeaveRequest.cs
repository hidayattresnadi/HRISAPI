using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.QueryParameter
{
    public class QueryParameterLeaveRequest
    {
        public string? OrderBy { get; set; } = "ProcessId";
        public bool Ascending { get; set; } = true;
        public string? LeaveType { get; set; }
        public string? EmployeeName { get; set; }
        public string? KeyWord { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
    }
}
