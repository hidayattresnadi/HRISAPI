using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Domain.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequestGroupDTO>> GetGroupedLeaveRequests(LeaveRequestDTOFiltered request);
        Task<(IEnumerable<LeaveRequest>, int totalCount)> GetAllLeaveRequestList(QueryParameterLeaveRequest request, Expression<Func<LeaveRequest, bool>>? expression);
    }
}
