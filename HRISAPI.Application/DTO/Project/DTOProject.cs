using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Project
{
    public class DTOProject
    {
        public int ProjNo { get; set; }
        [Required(ErrorMessage = "Project Name is required")]
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required and must be a positive integer")]
        public int DeptId { get; set; }
    }
}
