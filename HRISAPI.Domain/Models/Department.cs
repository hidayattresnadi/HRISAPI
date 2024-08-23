using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int Number {  get; set; }

        [ForeignKey("Employee")]
        public int? MgrEmpNo { get; set; }
        public virtual Employee Manager { get; set; }
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; }
        public virtual ICollection<Department_Location>? Locations { get; set; }
    }
}
