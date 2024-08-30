using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Infrastructure.Repositories
{
    public class DependentRepository : Repository<Dependent>, IDependentRepository
    {
        private readonly MyDbContext _db;
        public DependentRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Dependent>> GetAllDependentsAsync(List<String> roles,string? includeProperties = null, int? employeeId = null)
        {
            IQueryable<Dependent> query = _db.Dependents.AsQueryable();
            bool isAdmin = roles.Contains(Roles.Role_Administrator);
            bool isHRManager = roles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = roles.Contains(Roles.Role_Employee);

            if (isAdmin)
            {
            }
            else if (isHRManager)
            {
            }
            else if (isEmployee && employeeId.HasValue)
            {
                query = query.Where(d => d.EmployeeId == employeeId.Value);
            }
            // Include navigation properties
            if (!string.IsNullOrEmpty(includeProperties))
            {
                string[] includeProps = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includeProps)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }
        public Dependent Update(Dependent foundDependent, DependentDTO dependent)
        {
            foundDependent.BirthDate= dependent.BirthDate;
            foundDependent.Name = dependent.Name;
            foundDependent.Sex = dependent.Sex;
            foundDependent.Relations = dependent.Relations;
            foundDependent.EmployeeId = dependent.EmployeeId;
            return foundDependent;
        }
    }
}
