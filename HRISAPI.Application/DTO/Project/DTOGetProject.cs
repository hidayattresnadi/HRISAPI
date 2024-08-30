using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Project
{
    public class DTOGetProject
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
    }
}
