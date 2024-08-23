using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DTOEmployeeAdd
    {
        [Required(ErrorMessage = "Name is required")]
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "SSN is required")]
        public string SSN { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Salary must be more than 0")]
        public int Sallary { get; set; }
        [Required(ErrorMessage = "Sex is required")]
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }
        public int? DepartmentId { get; set; }
        [Required(ErrorMessage = "Employement Type is required")]
        public string EmploymentType { get; set; }
        public int Level { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Address is required")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Job Position Address is required")]
        public string JobPosition { get; set; }
        public int? SuperVisorId { get; set; }
        public List<DependentDTO>? Dependents { get; set; }
    }
}
