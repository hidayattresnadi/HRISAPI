using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class WorkflowSequence
    {
        [Key]
        public int StepId { get; set; }
        [ForeignKey("Workflow")]
        public int WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }
        public int StepOrder {  get; set; }
        public string StepName { get; set; }
        public string? RequiredRole { get; set; }
    }
}


//public class WorkflowSequence
//{
//    [Key]
//    public int StepId { get; set; }
//    [ForeignKey("Workflow")]
//    public int WorkflowId { get; set; }
//    // Relasi ke Workflow
//    public virtual Workflow Workflow { get; set; }
//    public int StepOrder { get; set; }
//    public string StepName { get; set; }
//    public string RequiredRole { get; set; }
//    // Relasi ke WorkflowActions
//    public ICollection<WorkflowAction> WorkflowActions { get; set; }
//}
