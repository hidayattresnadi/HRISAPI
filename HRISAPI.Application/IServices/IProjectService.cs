using HRISAPI.Application.DTO.Project;
using HRISAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.IServices
{
    public interface IProjectService
    {
        Task<DTOProject> AddProject(DTOProjectAdd inputProject);
        Task<IEnumerable<DTOGetProject>> GetAllProjects();
        Task<DTOGetProject> GetProjectByIdDetail(int id);
        Task<Project> UpdateProject(DTOProject project, int id);
        Task<bool> DeleteProject(int id);
    }
}
