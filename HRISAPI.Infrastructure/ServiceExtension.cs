using HRISAPI.Application.Repositories;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Infrastructure.Context;
using HRISAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRISAPI.Infrastructure
{
    public static class ServiceExtension
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MyDbContext>(options => options.UseNpgsql(connection));
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IProjectRepository,ProjectRepository>();
            services.AddScoped<IWorksOnRepository,WorksOnRepository>();
            services.AddScoped<IDependentRepository, DependentRepository>();
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowSequenceRepository, WorkflowSequenceRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IWorkflowActionRepository, WorkflowActionRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<INextStepRulesRepository, NextStepRulesRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        }
    }
}
