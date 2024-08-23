﻿using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Department;
using HRISAPI.Application.Exceptions;
using HRISAPI.Application.IServices;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public DepartmentService(IDepartmentRepository departmentRepository, ILocationRepository locationRepository, IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _locationRepository = locationRepository;
            _employeeRepository = employeeRepository;
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
        public async Task<IEnumerable<DTODepartment>> GetAllDepartments(QueryParameterDepartment? queryParameter)
        {
            var departments = await _departmentRepository.GetAllDepartmentsSorted("Manager", queryParameter);
            var departmentDtos = departments.Select(department => new DTODepartment
            {
                Name=department.Name,
                Number=department.Number,
                ManagerName=department.Manager.EmployeeName
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
            var foundDepartment = await GetDepartmentById(id);
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
