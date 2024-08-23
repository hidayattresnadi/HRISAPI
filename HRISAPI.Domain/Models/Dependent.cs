using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Dependent
    {
        [Key]
        public int DependentId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Relations { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
