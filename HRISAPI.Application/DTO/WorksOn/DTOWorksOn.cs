using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.WorksOn
{
    public class DTOWorksOn
    {
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeId is required and must be a positive integer")]
        public int EmpNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ProjectId is required and must be a positive integer")]
        public int ProjNo { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "HoursWorked is required and must be a positive integer")]
        public decimal Hoursworked { get; set; }
    }
}
