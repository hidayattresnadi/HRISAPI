using HRISAPI.Application.DTO.LeaveRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface ILeaveRequestService
    {
        Task<byte[]> GenerateLeaveRequestsPDF(LeaveRequestDTOFiltered request);
        Task<IEnumerable<LeaveRequestGroupDTO>> GetLeavesType(LeaveRequestDTOFiltered request);
    }
}
