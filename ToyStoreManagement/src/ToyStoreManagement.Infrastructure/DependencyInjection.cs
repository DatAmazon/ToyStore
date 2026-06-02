using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Interfaces;
using ToyStoreManagement.Infrastructure.ExternalServices;
using ToyStoreManagement.Infrastructure.Persistence;

namespace ToyStoreManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OracleDbConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString));

            // Cấu hình Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}
