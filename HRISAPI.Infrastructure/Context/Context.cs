using HRISAPI.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Infrastructure.Context
{
    public class MyDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorksOn> WorksOns { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department_Location> Department_Locations { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<WorkflowSequence> WorkflowSequences { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<WorkflowAction> WorkflowActions { get; set; }
        public DbSet<NextStepRules> NextStepsRules { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Location) // Setiap Project memiliki satu Location
                .WithMany(l => l.Projects) // Setiap Location memiliki banyak Projects
                .HasForeignKey(p => p.LocationId) // ForeignKey di Project adalah LocationId
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Process)
                .WithMany(p => p.LeaveRequests )
                .HasForeignKey(l => l.ProcessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkflowSequence>()
                .HasOne(ws => ws.Role) // Navigasi ke AspNetRoles
                .WithOne() // Tidak ada navigasi balik
                .HasForeignKey<WorkflowSequence>(ws => ws.RequiredRole) // Foreign key di WorkflowSequence
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithOne()
                .HasForeignKey<LeaveRequest>(l => l.EmployeeID) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.RequestId).HasName("request_pkey");
                entity.ToTable("request");
                entity.Property(e => e.RequestId).HasColumnName("id_request");
                entity.Property(e => e.ProcessName).HasMaxLength(255).HasColumnName("processname");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.StartDate).HasColumnName("startdate");
                entity.Property(e => e.EndDate).HasColumnName("enddate");
            });
            modelBuilder.Entity<Process>(entity =>
            {
                entity.HasKey(e => e.ProcessId).HasName("process_pkey");
                entity.ToTable("process");
                entity.Property(e => e.ProcessId).HasColumnName("id_process");
                entity.Property(e => e.RequestId).HasColumnName("id_request");
                entity.Property(e => e.WorkflowId).HasColumnName("id_workflow");
                entity.Property(e => e.RequesterId).HasColumnName("id_requester");
                entity.Property(e => e.RequestType).HasColumnName("request_type");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.CurrentStepId).HasColumnName("id_current_step");
                entity.Property(e => e.RequestDate).HasColumnName("request_date");
                entity.HasOne(p => p.Workflow)
                     .WithMany(w => w.Processes)
                     .HasForeignKey(p => p.WorkflowId)
                     .HasConstraintName("FK_Process_Workflow");
                entity.HasOne(p => p.Requester)
                     .WithMany(r => r.Processes)
                     .HasForeignKey(p => p.RequesterId)
                     .HasConstraintName("FK_Process_Requester");
                entity.HasOne(p => p.Request)
                     .WithOne(r => r.Process)
                     .HasForeignKey<Process>(p => p.RequestId)
                     .HasConstraintName("FK_Process_Request");
            });
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(e => e.WorkflowId).HasName("workflow_pkey");
                entity.ToTable("workflow");
                entity.Property(e => e.WorkflowId).HasColumnName("id_workflow");
                entity.Property(e => e.WorkflowName).HasMaxLength(255).HasColumnName("workflowname");
                entity.Property(e => e.Description).HasColumnName("description");
            });
            modelBuilder.Entity<WorkflowSequence>(entity =>
            {
                entity.HasKey(e => e.StepId).HasName("workflow_sequence_pkey");
                entity.ToTable("workflow_sequence");
                entity.Property(e => e.StepId).HasColumnName("step_id");
                entity.Property(e => e.WorkflowId).HasColumnName("id_workflow");
                entity.Property(e => e.StepOrder).HasColumnName("steporder");
                entity.Property(e => e.StepName).HasMaxLength(255).HasColumnName("stepname");
                entity.Property(e => e.RequiredRole).HasMaxLength(255).HasColumnName("requiredrole");
                entity.HasOne(d => d.Workflow).WithMany(p => p.WorkflowSequences)
                     .HasForeignKey(d => d.WorkflowId)
                     .HasConstraintName("workflow_sequence_id_workflow_fkey");
            });
            modelBuilder.Entity<WorkflowAction>(entity =>
            {
                entity.HasKey(e => e.ActionId).HasName("workflow_action_pkey");
                entity.ToTable("workflow_action");
                entity.Property(e => e.ActionId).HasColumnName("id_action");
                entity.Property(e => e.ProcessId).HasColumnName("id_proceess");
                entity.Property(e => e.StepId).HasColumnName("id_step");
                entity.Property(e => e.ActorId).HasColumnName("id_actor");
                entity.Property(e => e.Action).HasMaxLength(255).HasColumnName("action");
                entity.Property(e => e.ActionDate).HasColumnName("actiondate");
                entity.Property(e => e.Comments).HasColumnName("comments");
                entity.HasOne(wf => wf.Process).WithMany(p => p.WorkflowActions)
                     .HasForeignKey(wf => wf.ProcessId)
                     .HasConstraintName("workflow_action_id_request_fkey");
                entity.HasOne(e => e.Actor).WithMany(u => u.WorkflowActions)
                     .HasForeignKey(e => e.ActorId)
                     .HasConstraintName("FK_WorkflowAction_User");
            });
            modelBuilder.Entity<NextStepRules>(entity =>
            {
                entity.HasKey(e => e.RuleId).HasName("next_step_rule_pkey");
                entity.ToTable("next_step_rule");
                entity.Property(e => e.RuleId).HasColumnName("id_rule");
                entity.Property(e => e.CurrentStepId).HasColumnName("id_currentstep");
                entity.Property(e => e.NextStepId).HasColumnName("id_nextstep");
                entity.Property(e => e.ConditionType)
                     .HasMaxLength(100)
                     .HasColumnName("conditiontype");
                entity.Property(e => e.ConditionValue)
                     .HasMaxLength(255)
                     .HasColumnName("conditionvalue");
                entity.HasOne(d => d.CurrentStep)
                     .WithMany()
                     .HasForeignKey(d => d.CurrentStepId)
                     .HasConstraintName("next_step_rule_id_currentstep_fkey")
                     .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.NextStep)
                     .WithMany()
                     .HasForeignKey(d => d.NextStepId)
                     .HasConstraintName("next_step_rule_id_nextstep_fkey")
                     .OnDelete(DeleteBehavior.Restrict);
            });  
        }
    }
}
