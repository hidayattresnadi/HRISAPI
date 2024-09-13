using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Infrastructure.Repositories
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository 
    {
        private readonly MyDbContext _db;
        public LeaveRequestRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<IEnumerable<LeaveRequestGroupDTO>> GetGroupedLeaveRequests(LeaveRequestDTOFiltered request)
        {
            var leaveRequests = await _db.LeaveRequests.Include("Process").Where(l => l.Process.Status == "Accepted" && l.StartDate >= request.StartDate && l.EndDate <= request.EndDate)
                                .GroupBy(l => new { l.LeaveType })
                                .Select(g => new LeaveRequestGroupDTO
                                {
                                  LeaveType = g.Key.LeaveType,
                                  TotalLeaves = g.Count()
                                })
                                .ToListAsync();
            return leaveRequests;
        }
    }
}
