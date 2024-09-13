using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.WorksOn;
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
        public async Task<IEnumerable<MostProductiveEmployeesDTO>> GetMostProductiveEmployees()
        {
            var mosProductiveEmployees = await _db.WorksOns.Include("Employee").GroupBy(e => new { e.Employee.EmployeeName }).Select(g => new MostProductiveEmployeesDTO
            {
                EmployeeName = g.Key.EmployeeName,
                TotalHours = g.Sum(wo => wo.Hoursworked)
            }).OrderByDescending(mpe => mpe.TotalHours).Take(5).ToListAsync();
            return mosProductiveEmployees;
        }

        public async Task<IEnumerable<WorksOnProjectReport>> GetProjectReport()
        {
            var projectReports = await _db.WorksOns.Include(w=>w.Project)
                .GroupBy(w => new { w.Project.Name })
                .Select(g => new WorksOnProjectReport
                {
                    ProjectName = g.Key.Name,
                    AverageHours = g.Average(g => g.Hoursworked),
                    TotalEmployees = g.Count(),
                    TotalHours = g.Sum(g => g.Hoursworked)
                }).ToListAsync();
            return projectReports;
        }
    }
}
