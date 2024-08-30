using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.WorksOn
{
    public class DTOWorksOnDetail
    {
        public string EmpName { get; set; }
        public string ProjName { get; set; }
        public string DeptName { get; set; }
        public string SuperVisorName { get; set; }
        public decimal Hoursworked { get; set; }
    }
}
