using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.DTO.Department.HRISAPI.Application.DTO.Department;
using HRISAPI.Application.DTO.Dependent;
using HRISAPI.Application.DTO.Project;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.Services
{
    public class DependentService : IDependentService
    {
        private readonly IDependentRepository _dependentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DependentService(IDependentRepository dependentRepository, IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dependentRepository = dependentRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<DependentDTO> AddDependent(DependentDTO inputDependent)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            if (isAdmin)
            {

            }
            else if (isHRManager)
            {

            }
            else if (isEmployee && inputDependent.EmployeeId != intEmployeeId)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this dependent's information. Please ensure you have the correct permissions.");
            }

            if (!await _employeeRepository.AnyAsync(e => e.EmployeeId == inputDependent.EmployeeId))
            {
                throw new BadRequestException("Invalid Dependent ID");
            }

            var newDependent = new Dependent
            {
                EmployeeId = inputDependent.EmployeeId,
                BirthDate = inputDependent.BirthDate,
                Relations = inputDependent.Relations,
                Sex = inputDependent.Sex,
                Name = inputDependent.Name
            };
            await _dependentRepository.AddAsync(newDependent);
            await _dependentRepository.SaveAsync();

            var dependentDTO = new DependentDTO
            {
                EmployeeId = inputDependent.EmployeeId,
                BirthDate = inputDependent.BirthDate,
                Relations = inputDependent.Relations,
                Sex = inputDependent.Sex,
                Name = inputDependent.Name
            };
            return dependentDTO;
        }
        private async Task<Dependent> GetDependentById(List<String>userRoles, int? intEmployeeId, int id)
        {
            Dependent chosenDependent = await _dependentRepository.GetFirstOrDefaultAsync(foundDependent => foundDependent.DependentId == id,"Employee");
            if (chosenDependent == null)
            {
                throw new NotFoundException("Dependent is not found");
            }
            bool isAdmin = userRoles.Contains(Roles.Role_Administrator);
            bool isHRManager = userRoles.Contains(Roles.Role_HR_Manager);
            bool isEmployee = userRoles.Contains(Roles.Role_Employee);
            if (isAdmin)
            {

            }
            else if (isHRManager)
            {

            }
            else if (isEmployee && chosenDependent.EmployeeId != intEmployeeId)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this dependent's information. Please ensure you have the correct permissions.");
            }
            return chosenDependent;
        }
        public async Task<DependentDTODetail> GetDependentDetailById(int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            Dependent chosenDependent = await GetDependentById(userRoles,intEmployeeId,id);

            var dependentDTO = new DependentDTODetail
            {
                DependentId = id,
                EmployeeName = chosenDependent.Employee != null ? chosenDependent.Employee.EmployeeName : "No Employee",
                Sex = chosenDependent.Sex,
                BirthDate = chosenDependent.BirthDate,
                Relations = chosenDependent.Relations,
                Name = chosenDependent.Name
            };
            return dependentDTO;
        }
        public async Task<IEnumerable<DependentDTODetail>> GetAllDependents()
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var dependents = await _dependentRepository.GetAllDependentsAsync(userRoles,"Employee",intEmployeeId);
            var dependentDtos = dependents.Select(dependent => new DependentDTODetail
            {
                DependentId = dependent.DependentId,
                EmployeeName = dependent.Employee != null ? dependent.Employee.EmployeeName : "No Employee",
                Sex = dependent.Sex,
                BirthDate = dependent.BirthDate,
                Relations = dependent.Relations,
                Name = dependent.Name
            }).ToList();
            return dependentDtos;
        }
        public async Task<DependentDTODetail> UpdateDependent(DependentDTO dependent, int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            if (!await _employeeRepository.AnyAsync(e => e.EmployeeId == dependent.EmployeeId))
            {
                throw new BadRequestException("Invalid Dependent ID");
            }
            var foundDependent = await GetDependentById(userRoles,intEmployeeId,id);
            var updatedDependent = _dependentRepository.Update(foundDependent, dependent);
            await _dependentRepository.SaveAsync();
            var updatedDependentDTO = new DependentDTODetail
            {
                DependentId = id,
                EmployeeName = foundDependent.Employee != null ? foundDependent.Employee.EmployeeName : "No Employee",
                Sex = foundDependent.Sex,
                BirthDate = foundDependent.BirthDate,
                Relations = foundDependent.Relations,
                Name = foundDependent.Name
            };
            return updatedDependentDTO;
        }

        public async Task<bool> DeleteDependent(int id)
        {
            var employeeId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            int? intEmployeeId = string.IsNullOrEmpty(employeeId) ? (int?)null : int.Parse(employeeId);
            var userRoles = _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            var foundDependent = await GetDependentById(userRoles,intEmployeeId,id);
            _dependentRepository.Remove(foundDependent);
            await _dependentRepository.SaveAsync();
            return true;
        }
    }
}
