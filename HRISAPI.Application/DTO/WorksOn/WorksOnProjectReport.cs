using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.WorksOn
{
    public class WorksOnProjectReport
    {
        public string ProjectName { get; set; }
        public decimal TotalHours { get; set; }
        public int TotalEmployees { get; set; }
        public decimal AverageHours { get; set; }  
    }
}
