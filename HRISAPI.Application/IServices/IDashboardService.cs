using HRISAPI.Application.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardInfo();
    }
}
