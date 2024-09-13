using HRISAPI.Application.DTO.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.Dashboard
{
    public class DashboardDTO
    {
        public IEnumerable<EmployeeDistributionDTO> EmployeeDistributions { get; set; }
        public IEnumerable<MostProductiveEmployeesDTO> MostProductiveEmployees { get; set; }
        public IEnumerable<DepartmentSallaryDTO> DepartmentSallaries { get; set; }
        public IEnumerable<ProcessDetailDTO> ProcessFollowUp {  get; set; }
    }
}
