using HRISAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Infrastructure.Context
{
    public class MyDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorksOn> WorksOns { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department_Location> Department_Locations { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-One: Department has one Manager (Employee)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithOne()
                .HasForeignKey<Department>(d => d.MgrEmpNo)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Department has many Employees
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Department has many Projects
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Projects)
                .WithOne(p => p.Department)
                .HasForeignKey(p => p.DeptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: Employee <-> Project via WorksOn
            modelBuilder.Entity<WorksOn>()
                .HasKey(wo => new { wo.EmpNo, wo.ProjNo }); // Composite key for EmployeeId and ProjectId

            modelBuilder.Entity<WorksOn>()
                .HasOne(wo => wo.Employee)
                .WithMany(e => e.WorksOns)
                .HasForeignKey(wo => wo.EmpNo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorksOn>()
                .HasOne(wo => wo.Project)
                .WithMany(p => p.WorksOnProjects)
                .HasForeignKey(wo => wo.ProjNo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Dependents)
                .WithOne(d => d.Employee)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department_Location>()
            .HasOne(dl => dl.Department)
            .WithMany(d => d.Locations)
            .HasForeignKey(dl => dl.DepartmentId);

            modelBuilder.Entity<Department_Location>()
                .HasOne(dl => dl.Location)
                .WithMany(l => l.Departments)
                .HasForeignKey(dl => dl.LocationId);
        }

    }
}
