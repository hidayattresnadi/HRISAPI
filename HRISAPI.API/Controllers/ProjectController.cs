using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Project;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRISAPI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager)]
        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody] DTOProjectAdd project)
        {
            var inputProject = await _projectService.AddProject(project);
            return Ok(inputProject);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjects();
            return Ok(projects);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            DTOGetProject project = await _projectService.GetProjectByIdDetail(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProject([FromBody] DTOProject project, int id)
        {
            
                var updatedProject = await _projectService.UpdateProject(project, id);
                if (updatedProject == null)
                {
                    return NotFound();
                }
                return Ok(updatedProject);
        }
        [Authorize(Roles = Roles.Role_Administrator + "," + Roles.Role_Department_Manager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            bool isDeletedProject = await _projectService.DeleteProject(id);
            if (isDeletedProject == false)
            {
                return NotFound();
            }
            return Ok("Project is deleted");
        }
    }
}
