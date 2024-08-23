using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO
{
    public class DTOUpdatedEmployee
    {
        public string EmployeeName { get; set; }
        public string SSN { get; set; }
        public string Address { get; set; }
        public int Sallary { get; set; }
        public string Sex { get; set; }
        public DateOnly BirthDate { get; set; }
        public int? DepartmentId { get; set; }
        public string EmploymentType { get; set; }
        public int Level { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string JobPosition { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int? SuperVisorId { get; set; }
    }
}
