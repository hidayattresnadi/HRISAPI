﻿using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequestGroupDTO>> GetGroupedLeaveRequests(LeaveRequestDTOFiltered request);
    }
}
