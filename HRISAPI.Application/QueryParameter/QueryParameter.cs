using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.QueryParameter
{
    public class QueryParameter
    {
        public string? OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
        public string? EmployeeType {  get; set; }
        public int? EmployeeLevel { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeName { get; set; }
        public string? JobPosition { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
