using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Department_Location
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Department Department { get; set; }
        public virtual Location Location { get; set; }
    }
}
