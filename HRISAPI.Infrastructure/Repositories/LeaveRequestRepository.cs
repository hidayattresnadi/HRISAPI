using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<(IEnumerable<LeaveRequest>, int totalCount)> GetAllLeaveRequestList(QueryParameterLeaveRequest request, Expression<Func<LeaveRequest, bool>>? expression)
        {
            var entities = _db.LeaveRequests.AsQueryable();

            // Include navigational properties
            entities = entities.Include(lr => lr.Process).ThenInclude(p => p.Requester).ThenInclude(r =>r.Employee);
            entities = entities.Include(lr => lr.Process).ThenInclude(p => p.WorkflowSequence);

            // Apply filter expression if provided
            if (expression != null)
            {
                entities = entities.Where(expression);
            }

            // Apply sorting (primary sorting by user input)
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                entities = request.Ascending
                    ? entities.OrderBy(e => EF.Property<object>(e, request.OrderBy))
                    : entities.OrderByDescending(e => EF.Property<object>(e, request.OrderBy));
            }

            // Sorting by ProcessId as fallback if no OrderBy provided
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                entities = entities.OrderBy(lr => lr.ProcessId);
            }

            // Count total items before pagination
            int totalCount = await entities.CountAsync();

            // Default PageSize if 0
            if (request.PageSize <= 0)
            {
                request.PageSize = totalCount;
            }

            // Apply pagination after sorting
            entities = entities.Skip((request.PageNumber - 1) * request.PageSize)
                               .Take(request.PageSize);

            // Fetch results asynchronously
            var leaveRequests = await entities.ToListAsync();

            return (leaveRequests, totalCount);
        }

    }
}
