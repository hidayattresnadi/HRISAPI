using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Domain.IRepositories
{
    public interface IProcessRepository : IRepository<Process>
    {
        Task<IEnumerable<Process>> GetProcessBasedOnRole(List<string> roles);
    }
}
