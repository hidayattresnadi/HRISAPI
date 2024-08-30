using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.User
{
    public class ModifyUserRole
    {
        public List<string>? RolesToAdd { get; set; }
        public List<string>? RolesToRemove { get; set; }
    }
}
