using HRISAPI.Application.Repositories;
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
        }
    }
}
