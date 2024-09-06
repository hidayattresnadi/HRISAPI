using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;
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
    }
}
