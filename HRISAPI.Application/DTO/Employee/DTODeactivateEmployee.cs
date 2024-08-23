using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DTODeactivateEmployee
    {
        public string DeleteReasoning { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
