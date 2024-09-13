using HRISAPI.Application.DTO;
using HRISAPI.Application.DTO.Dashboard;
using HRISAPI.Application.DTO.Employee;
using HRISAPI.Application.QueryParameter;
using HRISAPI.Application.Repositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Infrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly MyDbContext _db;
        public EmployeeRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
        public Employee Update(Employee foundEmployee, DTOEmployeeAdd employee)
        {
            var today = DateTime.UtcNow;
            foundEmployee.Address = employee.Address;
            foundEmployee.Sex = employee.Sex;
            foundEmployee.Sallary = employee.Sallary;
            foundEmployee.EmailAddress = employee.EmailAddress;
            foundEmployee.BirthDate = employee.BirthDate;
            foundEmployee.LastUpdatedDate = today;
            foundEmployee.EmployeeName = employee.EmployeeName;
            foundEmployee.JobPosition = employee.JobPosition;
            foundEmployee.Level = employee.Level;
            foundEmployee.SSN = employee.SSN;
            foundEmployee.DepartmentId = employee.DepartmentId;
            foundEmployee.PhoneNumber = employee.PhoneNumber;
            foundEmployee.SuperVisorId = employee.SuperVisorId;
            foundEmployee.EmploymentType = employee.EmploymentType;
            return foundEmployee;
        }
        public Employee UpdateForEmployee(Employee foundEmployee, DTOEmployeeAdd employee)
        {
            var today = DateTime.UtcNow;
            foundEmployee.Address = employee.Address;
            foundEmployee.Sex = employee.Sex;
            foundEmployee.EmailAddress = employee.EmailAddress;
            foundEmployee.BirthDate = employee.BirthDate;
            foundEmployee.LastUpdatedDate = today;
            foundEmployee.EmployeeName = employee.EmployeeName;
            foundEmployee.JobPosition = employee.JobPosition;
            foundEmployee.Level = employee.Level;
            foundEmployee.DepartmentId = employee.DepartmentId;
            foundEmployee.PhoneNumber = employee.PhoneNumber;
            foundEmployee.SuperVisorId = employee.SuperVisorId;
            foundEmployee.EmploymentType = employee.EmploymentType;
            return foundEmployee;
        }

        public async Task<IEnumerable< Employee>> GetAllEmployeesSorted(string? includeProperties = null, QueryParameter? queryParameter = null)
        {
            var query = _db.Employees.AsQueryable();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                string[] includeProps = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includeProps)
                {
                    query = query.Include(include);
                }
            }
            if (!string.IsNullOrEmpty(queryParameter.EmployeeName))
            {
                query = query.Where(e => e.EmployeeName.ToLower().Contains(queryParameter.EmployeeName.ToLower()));
            }
            if (!string.IsNullOrEmpty(queryParameter.EmployeeType))
            {
                query = query.Where(e => e.EmploymentType.ToLower().Contains(queryParameter.EmployeeType.ToLower()));
            }
            if (queryParameter.EmployeeLevel.HasValue)
            {
                query = query.Where(e => e.Level == queryParameter.EmployeeLevel.Value);
            }
            if (!string.IsNullOrEmpty(queryParameter.DepartmentName))
            {
                query = query.Where(e => e.Department.Name.ToLower().Contains(queryParameter.DepartmentName.ToLower()));
            }
            if (!string.IsNullOrEmpty(queryParameter.JobPosition))
            {
                query = query.Where(e => e.JobPosition.ToLower().Contains(queryParameter.JobPosition.ToLower()));
            }
            if (!string.IsNullOrEmpty(queryParameter.OrderBy))
            {
                if (queryParameter.Ascending)
                {
                    query = queryParameter.OrderBy switch
                    {
                        "EmployeeName" => query.OrderBy(e => e.EmployeeName),
                        "Level" => query.OrderBy(e => e.Level),
                        "DepartmentName" => query.OrderBy(e => e.Department.Name),
                        "JobPosition" => query.OrderBy(e => e.JobPosition),
                        "LastUpdated" => query.OrderBy(e => e.LastUpdatedDate),
                        "EmploymentType" => query.OrderBy(e => e.EmploymentType),
                        _ => query.OrderBy(e => e.EmployeeId)
                    };
                }
                else
                {
                    query = queryParameter.OrderBy switch
                    {
                        "EmployeeName" => query.OrderByDescending(e => e.EmployeeName),
                        "Level" => query.OrderByDescending(e => e.Level),
                        "EmploymentType" => query.OrderByDescending(e => e.EmploymentType),
                        "DepartmentName" => query.OrderByDescending(e => e.Department.Name),
                        "JobPosition" => query.OrderByDescending(e => e.JobPosition),
                        "LastUpdated" => query.OrderByDescending(e => e.LastUpdatedDate),
                        _ => query.OrderByDescending(e => e.EmployeeId)
                    };
                }
            }
            query = query.Skip((queryParameter.PageNumber - 1) * queryParameter.PageSize).Take(queryParameter.PageSize);
            return await query.ToListAsync();
        }
        public async Task<Employee> DeactivateEmployee(Employee foundEmployee, string deleteReasoning)
        {
            var today = DateTime.UtcNow;
            foundEmployee.LastUpdatedDate = today;
            foundEmployee.Status = "Not Active";
            foundEmployee.DeactivationReasoning = deleteReasoning;
            return foundEmployee;
        }
        public async Task<Employee> AssignEmployeeToDepartment(Employee foundEmployee, int id)
        {
            var today = DateTime.UtcNow;
            foundEmployee.LastUpdatedDate = today;
            foundEmployee.DepartmentId = id;
            return foundEmployee;
        }

        public async Task<IEnumerable<EmployeeDistributionDTO>> GetEmployeesDistribution()
        {
            var totalDistributionEmployees = await _db.Employees.Include("Department").GroupBy(e => new { e.Department.Name }).Select(g => new EmployeeDistributionDTO
            {
                DepartmentName = g.Key.Name,
                DepartmentCount = g.Count()
            })
            .ToListAsync();
            return totalDistributionEmployees;
        }
        public async Task<IEnumerable<DepartmentSallaryDTO>> GetDepartmentSallaries()
        {
            var totalSallariesDepartments = await _db.Employees.Include("Department").GroupBy(e => new { e.Department.Name }).Select(g => new DepartmentSallaryDTO
            {
                DepartmentName = g.Key.Name,
                DepartmentSallary = g.Average(e => e.Sallary)
            })
           .ToListAsync();
            return totalSallariesDepartments;
        }
    }
}
