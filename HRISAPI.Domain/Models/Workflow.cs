using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Workflow
    {
        [Key]
        public int WorkflowId { get; set; }
        public string WorkflowName { get; set; }
        public string Description { get; set; }
        // Relasi ke Request
        public ICollection<Process> Processes { get; set; } = new List<Process>();
        public ICollection<WorkflowSequence> WorkflowSequences { get; set; } = new List<WorkflowSequence>();
    }
}
