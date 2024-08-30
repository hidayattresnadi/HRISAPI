using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Project;
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
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly MyDbContext _db;
        public ProjectRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<List<Project>> GetAllProjectsAsync(List<String> userRoles,int? intEmployeeId)
        {
            IQueryable<Project> query = _db.Projects
               .Include(p => p.Department)
               .Include(p => p.Location)
               .Include(p => p.WorksOnProjects).ThenInclude(w => w.Employee);

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            bool isEmployeeSupervisor = userRoles.Contains(Roles.Role_Employee_Supervisor);
            if (isAdmin || isHRManager)
            {
             
            }
            else if (isDepartmentManager)
            {
                query = query.Where(p => p.Department.Employees.Any(e => e.EmployeeId == intEmployeeId.Value));
            }
            else if (isEmployeeSupervisor)
            {
                query = query.Where(p => p.Department.Employees.Any(e => e.SuperVisorId == intEmployeeId.Value));
            }
            else if (isEmployee)
            {
                query = query.Where(p => p.WorksOnProjects.Any(w => w.EmpNo == intEmployeeId.Value));
            }
            return await query.ToListAsync();
        }
        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            return await _db.Projects
               .Include(p => p.Department) 
               .Include(p => p.Location) 
               .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public Project Update(Project foundProject, DTOProject project)
        {
            foundProject.Name = project.Name;
            foundProject.DeptId = project.DeptId;
            return foundProject;
        }
    }
}
