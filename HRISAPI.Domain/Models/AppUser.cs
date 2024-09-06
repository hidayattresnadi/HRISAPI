using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRISAPI.Domain.Models
{
    public class AppUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpire { get; set; }
        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<WorkflowAction> WorkflowActions { get; set; } = new List<WorkflowAction>();
        public virtual ICollection<Process> Processes { get; set; } = new List<Process>();
    }
}
