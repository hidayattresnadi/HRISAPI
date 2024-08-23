using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.QueryParameter;
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
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        private readonly MyDbContext _db;
        public DepartmentRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
        public Department Update(Department foundDepartment, DTOResultDepartmentAdd department)
        {
            foundDepartment.MgrEmpNo = department.MgrEmpNo;
            foundDepartment.Number = department.Number;
            foundDepartment.Name = department.Name;
            return foundDepartment;
        }
        public async Task<IEnumerable<Department>> GetAllDepartmentsSorted(QueryParameterDepartment? queryParameter = null)
        {
            var query = _db.Departments.AsQueryable();
            query = query.Include(d => d.Manager).Include(d => d.Locations).ThenInclude(dl => dl.Location);
            if (!string.IsNullOrEmpty(queryParameter.DepartmentName))
            {
                query = query.Where(d => d.Name.ToLower().Contains(queryParameter.DepartmentName.ToLower()));
            }
            if (!string.IsNullOrEmpty(queryParameter.OrderBy))
            {
                if (queryParameter.Ascending)
                {
                    query = queryParameter.OrderBy switch
                    {
                        "DepartmentName" => query.OrderBy(d => d.Name)
                    };
                }
                else
                {
                    query = queryParameter.OrderBy switch
                    {
                        "DepartmentName" => query.OrderByDescending(d => d.Name),
                    };
                }
            }
            query = query.Skip((queryParameter.PageNumber - 1) * queryParameter.PageSize).Take(queryParameter.PageSize);
            return await query.ToListAsync();
        }
    }
}
