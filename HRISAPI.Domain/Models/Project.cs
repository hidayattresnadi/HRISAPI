using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public string Name { get; set; }
        [ForeignKey("Department")]
        public int DeptId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<WorksOn>? WorksOnProjects { get; set; }
    }
}
