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
    public class Request
    {
        [Key]
        public int RequestId { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public virtual Process Process { get; set; }
    }
}



//public class Request
//{
//    [Key]
//    public int RequestId { get; set; }
//    [ForeignKey("Workflow")]
//    public int WorkflowId { get; set; }
//    public virtual Workflow Workflow { get; set; }
//    [ForeignKey("AspNetUsers")]
//    public string RequesterId { get; set; }
//    public virtual AppUser User { get; set; }
//    public string RequestType { get; set; }
//    public string Status { get; set; }
//    [ForeignKey("WorkflowSequence")]
//    public int CurrentStepId { get; set; }
//    public DateTime RequestDate { get; set; }
//    [ForeignKey("Process")]
//    public int ProcessId { get; set; }
//    public virtual Process Process { get; set; }
//    // Relasi ke WorkflowActions
//    public ICollection<WorkflowAction> WorkflowActions { get; set; } = new List<WorkflowAction>();
//}
