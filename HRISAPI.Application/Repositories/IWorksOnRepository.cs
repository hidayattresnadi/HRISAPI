using HRISAPI.Application.DTO.WorksOn;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface IWorksOnRepository : IRepository<WorksOn>
    {
        public WorksOn Update(WorksOn foundWorksOn, DTOWorksOn worksOn);
    }
}
