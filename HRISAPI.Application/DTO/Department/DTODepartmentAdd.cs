using HRISAPI.Application.DTO.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DTODepartmentAdd
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public int? MgrEmpNo { get; set; }
        public List<LocationDTO>? Locations { get; set; }
    }
}
