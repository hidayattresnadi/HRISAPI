using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DependentDTO
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Relations { get; set; }
        public int EmployeeId { get; set; }
    }
}
