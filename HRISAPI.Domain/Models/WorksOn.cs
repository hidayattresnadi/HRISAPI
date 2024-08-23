using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class WorksOn
    {
        [Key]
        public int WorksOnId { get; set; }
        [ForeignKey("Employee")]
        public int EmpNo { get; set; }
        public virtual Employee Employee { get; set; }
        [ForeignKey("Project")]
        public int ProjNo { get; set; }
        public virtual Project Project { get; set; }
        public decimal Hoursworked { get; set; }
    }
}
