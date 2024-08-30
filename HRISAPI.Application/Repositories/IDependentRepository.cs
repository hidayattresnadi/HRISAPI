using HRISAPI.Application.DTO;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface IDependentRepository : IRepository<Dependent>
    {
        Dependent Update(Dependent foundEmployee, DependentDTO dependent);
        Task<IEnumerable<Dependent>> GetAllDependentsAsync(List<String> roles, string? includeProperties = null, int? employeeId = null);
    }
}
