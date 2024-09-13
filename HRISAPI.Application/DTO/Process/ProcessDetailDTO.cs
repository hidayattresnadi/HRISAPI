using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Process
{
    public class ProcessDetailDTO
    {
        public int ProcessId { get; set; }
        public string WorkflowName { get; set; }
        public string Requester { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string CurrentStep { get; set; }
    }
}
