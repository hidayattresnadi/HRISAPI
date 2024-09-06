using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public int ProcessId { get; set; }
    }
}
