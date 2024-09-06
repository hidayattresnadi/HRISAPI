using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class WorkflowAction
    {
        [Key]
        public int ActionId { get; set; }
        [ForeignKey("Process")]
        public int ProcessId { get; set; }
        public virtual Process Process { get; set; }
        [ForeignKey("WorkflowSequence")]
        public int StepId { get; set; }
        public virtual WorkflowSequence WorkflowSequence { get; set; }
        [ForeignKey("AspNetUsers")]
        public string ActorId { get; set; }
        public virtual AppUser Actor { get; set; }
        public string Action {  get; set; }
        public DateTime ActionDate { get; set; }
        public string Comments { get; set; }
    }
}

//public class WorkflowAction
//{
//    [Key]
//    public int ActionId { get; set; }
//    [ForeignKey("Request")]
//    public int RequestId { get; set; }
//    public virtual Request Request { get; set; }
//    [ForeignKey("WorkflowSequence")]
//    public int StepId { get; set; }
//    public virtual WorkflowSequence WorkflowSequence { get; set; }
//    [ForeignKey("AspNetUsers")]
//    public string ActorId { get; set; }
//    public virtual AppUser Actor { get; set; }
//    public string Action { get; set; }
//    public DateTime ActionDate { get; set; }
//    public string Comments { get; set; }
//}
