using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Project;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRISAPI.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProjectService(IProjectRepository projectRepository, IDepartmentRepository departmentRepository,ILocationRepository locationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _projectRepository = projectRepository;
            _departmentRepository = departmentRepository;
            _locationRepository = locationRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> ValidateDupplicateProjectName(string projectName)
        {
            bool isDupplicate = await _projectRepository.AnyAsync(p => p.Name == projectName);
            return isDupplicate;
        }
        public async Task<DTOProject> AddProject(DTOProjectAdd inputProject)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);

            if (isAdmin)
            {

            }
            else if (isDepartmentManager)
            {
                var department = _departmentRepository.GetFirstOrDefaultAsync(d => d.MgrEmpNo == intEmployeeId);
                if (inputProject.DeptId != department?.Id) 
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }

            if (!await _departmentRepository.AnyAsync(d => d.DepartmentId == inputProject.DeptId))
            {
                throw new BadRequestException("Invalid Department ID");
            }
            int? locationId = null;
            if (!string.IsNullOrWhiteSpace(inputProject.LocationName))
            {
                var locationNameLower = inputProject.LocationName.ToLower();
                var existingLocation = await _locationRepository.GetFirstOrDefaultAsync(
           l => l.Name.ToLower() == locationNameLower);

                if (existingLocation != null)
                {
                    locationId = existingLocation.LocationId;
                }
                else
                {
                    var newLocation = new Location
                    {
                        Name = inputProject.LocationName
                    };
                    await _locationRepository.AddAsync(newLocation);
                    await _locationRepository.SaveAsync();

                    locationId = newLocation.LocationId;
                }
            }

            bool isDupplicate = await ValidateDupplicateProjectName(inputProject.Name);
            if (isDupplicate)
            {
                throw new BadRequestException("Project Name is Already Exist");
            }

            var newProject = new Project
            {
                DeptId = inputProject.DeptId,
                Name = inputProject.Name,
                LocationId = locationId
            };
            await _projectRepository.AddAsync(newProject);
            await _projectRepository.SaveAsync();

            var projectDTO = new DTOProject
            {
                ProjNo = newProject.ProjectId,
                Name = newProject.Name,
                DeptId = newProject.DeptId
            };
            return projectDTO;
        }

        public async Task<IEnumerable<DTOGetProject>> GetAllProjects()
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            var projects = await _projectRepository.GetAllProjectsAsync(userRoles,intEmployeeId);
            var projectDTOs = projects.Select(p => new DTOGetProject
            {
                ProjectId = p.ProjectId,
                Name = p.Name,
                LocationName = p.Location?.Name,
                DepartmentName = p.Department?.Name 
            }).ToList();
            return projectDTOs;
        }
        private async Task<Project> GetProjectById(List<String> userRoles, int? intEmployeeId, int id)
        {
            Project chosenProject = await _projectRepository.GetFirstOrDefaultAsync(foundProject => foundProject.ProjectId == id);
            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);
            if (isAdmin)
            {

            }
            else if (isDepartmentManager)
            {
                var department = _departmentRepository.GetFirstOrDefaultAsync(d => d.MgrEmpNo == intEmployeeId);
                if (chosenProject.DeptId != department?.Id)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            return chosenProject;
        }
        public async Task<DTOGetProject> GetProjectByIdDetail(int id)
        {
            Project chosenProject = await _projectRepository.GetProjectByIdAsync(id);
            var projectDTO = new DTOGetProject
            {
                ProjectId = chosenProject.ProjectId,
                Name = chosenProject.Name,
                LocationName = chosenProject.Location != null ? chosenProject.Location.Name : "No Location",
                DepartmentName = chosenProject.Department != null ? chosenProject.Department.Name : "No Department"
            };
            return projectDTO;
        }
        public async Task<Project> UpdateProject(DTOProject project, int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            if (!await _departmentRepository.AnyAsync(d => d.DepartmentId == project.DeptId))
            {
                throw new BadRequestException("Invalid Department ID");
            }
            var foundProject = await GetProjectById(userRoles,intEmployeeId,id);
            if (foundProject is null)
            {
                return null;
            }
            bool isDupplicate = await ValidateDupplicateProjectName(project.Name);
            if (isDupplicate)
            {
                throw new BadRequestException("Project Name is Already Exist");
            }
            _projectRepository.Update(foundProject, project);
            await _projectRepository.SaveAsync();
            return foundProject;
        }
        public async Task<bool> DeleteProject(int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            var foundProject = await GetProjectById(userRoles,intEmployeeId,id);
            if (foundProject is null)
            {
                return false;
            }
            _projectRepository.Remove(foundProject);
            await _projectRepository.SaveAsync();
            return true;
        }
    }
}
