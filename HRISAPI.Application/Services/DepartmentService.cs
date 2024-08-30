using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.DTO.Department.HRISAPI.Application.DTO.Department;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HRISAPI.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DepartmentService(IDepartmentRepository departmentRepository, ILocationRepository locationRepository, IEmployeeRepository employeeRepository,IHttpContextAccessor httpContextAccessor)
        {
            _departmentRepository = departmentRepository;
            _locationRepository = locationRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<DTOResultDepartmentAdd> AddDepartment(DTODepartmentAdd inputDepartment)
        {
            if (inputDepartment.MgrEmpNo != null) 
            {
                var mgrEmployee = await _employeeRepository.GetFirstOrDefaultAsync(e => e.EmployeeId == inputDepartment.MgrEmpNo);
                if (mgrEmployee == null)
                {
                    throw new NotFoundException("Manager Employee is not found");
                }
            }
            var locations = new List<Location>();
            foreach (var locationDto in inputDepartment.Locations)
            {
                var existingLocation = await _locationRepository.GetFirstOrDefaultAsync(l => l.Name == locationDto.Name);
                if (existingLocation != null)
                {
                    locations.Add(existingLocation);
                }
                else
                {
                    var newLocation = new Location { Name = locationDto.Name };
                    locations.Add(newLocation);
                }
            }
            var newDepartment = new Department
            {
                Name = inputDepartment.Name,
                Number = inputDepartment.Number,
                MgrEmpNo = inputDepartment.MgrEmpNo,
                Locations = locations.Select(location => new Department_Location { Location = location }).ToList()
            };
            await _departmentRepository.AddAsync(newDepartment);
            await _departmentRepository.SaveAsync();
            var dtoDepartment = new DTOResultDepartmentAdd
            {
                Name = newDepartment.Name,
                Number = newDepartment.Number,
                MgrEmpNo = newDepartment.MgrEmpNo,
            };
            return dtoDepartment;
        }
        public async Task<IEnumerable<DTODepartmentLocation>> GetAllDepartments(QueryParameterDepartment? queryParameter)
        {
            var departments = await _departmentRepository.GetAllDepartmentsSorted(queryParameter);
            var departmentDtos = departments.Select(department => new DTODepartmentLocation
            {
                DepartmentID = department.DepartmentId,
                DepartmentName=department.Name,
                Number=department.Number,
                ManagerName= department.Manager != null ? department.Manager.EmployeeName : "No Manager",
                LocationNames = department.Locations != null
                                ? department.Locations.Select(dl => dl.Location.Name).ToList()
                                : new List<string>()
            }).ToList();
            return departmentDtos;
        }
        public async Task<Department> GetDepartmentById(int id)
        {
            Department chosenDepartment = await _departmentRepository.GetFirstOrDefaultAsync(foundDepartment => foundDepartment.DepartmentId == id);
            if (chosenDepartment == null)
            {
                throw new NotFoundException("Department is not found");
            }
            return chosenDepartment;
        }
        public async Task<DTODepartment> GetDepartmentDetailById(int id)
        {
            Department chosenDepartment = await _departmentRepository.GetFirstOrDefaultAsync((foundDepartment => foundDepartment.DepartmentId == id), "Manager");
            if (chosenDepartment == null)
            {
                throw new NotFoundException("Department is not found");
            }
            var departmentDTO = new DTODepartment
            {
                Name=chosenDepartment.Name,
                Number=chosenDepartment.Number,
                ManagerName = chosenDepartment.Manager != null ? chosenDepartment.Manager.EmployeeName : "No Manager"
            };
            return departmentDTO;
        }
        public async Task<DTOResultDepartmentAdd> UpdateDepartment(DTOResultDepartmentAdd department, int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isDepartmentManager = userRoles.Contains(Roles.Role_Department_Manager);

            var foundDepartment = await GetDepartmentById(id);
            if (isAdmin)
            {

            }
            else if (isDepartmentManager)
            {
                if (intEmployeeId != foundDepartment.MgrEmpNo)
                {
                    throw new UnauthorizedAccessException("You are not authorized. Please ensure you have the correct permissions.");
                }
            }
            var updatedDepartment = _departmentRepository.Update(foundDepartment, department);
            await _employeeRepository.SaveAsync();
            var updatedDepartmentDTO = new DTOResultDepartmentAdd
            {
                MgrEmpNo = updatedDepartment.MgrEmpNo,
                Name = updatedDepartment.Name,
                Number = updatedDepartment.Number,
            };
            return updatedDepartmentDTO;
        }

        public async Task<bool> DeleteDepartment(int id)
        {
            var foundDepartment = await GetDepartmentById(id);
            _departmentRepository.Remove(foundDepartment);
            await _departmentRepository.SaveAsync();
            return true;
        }
    }
}
