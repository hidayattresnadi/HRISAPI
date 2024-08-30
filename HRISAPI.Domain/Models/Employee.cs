using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string SSN {  get; set; }
        public string Address {  get; set; }
        public int Sallary { get; set; }
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public string EmploymentType { get; set; }
        public int Level { get; set; }
        public string JobPosition { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Status  { get; set; }
        public string? DeactivationReasoning {  get; set; }
        public int? SuperVisorId { get; set; }
        public virtual Employee? Supervisor { get; set; }
        public virtual ICollection<Employee>? Supervisees { get; set; }
        public virtual ICollection<Dependent>? Dependents { get; set; }
        public virtual ICollection<WorksOn>? WorksOns { get; set; }
        public virtual AppUser AppUser { get; set; }

    }
}
