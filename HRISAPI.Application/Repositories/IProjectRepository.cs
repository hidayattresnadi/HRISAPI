using HRISAPI.Application.DTO.Project;
using HRISAPI.Application.DTO.WorksOn;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<List<Project>> GetAllProjectsAsync(List<string>userRoles,int? intEmployeeId);
        Task<Project> GetProjectByIdAsync(int id);
        Project Update(Project foundProject, DTOProject project);
    }
}
