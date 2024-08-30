using HRISAPI.Application.DTO.WorksOn;
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
    public class WorksOnRepository : Repository<WorksOn>, IWorksOnRepository
    {
        private readonly MyDbContext _db;
        public WorksOnRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }

        public WorksOn Update(WorksOn foundWorksOn, DTOWorksOn worksOn)
        {
            foundWorksOn.ProjNo = worksOn.ProjNo;
            foundWorksOn.EmpNo = worksOn.EmpNo;
            foundWorksOn.Hoursworked = worksOn.Hoursworked;
            return foundWorksOn;
        }
    }
}
