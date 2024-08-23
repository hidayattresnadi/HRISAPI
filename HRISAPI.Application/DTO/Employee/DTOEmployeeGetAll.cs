using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DTOEmployeeGetAll
    {
        public string EmployeeName { get; set; }
        public string Department {  get; set; }
        public string JobPosition { get; set; }
        public int Level { get; set; }
        public string EmploymentType { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
