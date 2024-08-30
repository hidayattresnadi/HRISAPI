using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Dependent
{
    public class DependentDTODetail
    {
        public int DependentId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Relations { get; set; }
        public string EmployeeName { get; set; }
    }
}
