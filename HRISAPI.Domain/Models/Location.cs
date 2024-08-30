using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Department_Location> Departments { get; set; } = new List<Department_Location>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
