using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Department
{
    namespace HRISAPI.Application.DTO.Department
    {
        public class DTODepartmentLocation
        {
            public int DepartmentID { get; set; }
            public string DepartmentName { get; set; }
            public int Number { get; set; }
            public string ManagerName { get; set; }
            public List<string> LocationNames { get; set; }
        }
    }
}
