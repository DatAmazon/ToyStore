using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Interfaces;
using ToyStoreManagement.Infrastructure.ExternalServices;
using ToyStoreManagement.Infrastructure.Persistence;
using ToyStoreManagement.Application.DTOs.Common;

namespace ToyStoreManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string webRootPath)
        {
            var connectionString = configuration.GetConnectionString("OracleDbConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString));

            // Cấu hình MinIO
            services.Configure<MinioSettings>(configuration.GetSection("Minio"));
            services.AddScoped<IStorageService, MinioStorageService>();

            // Cấu hình Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IReportService>(provider => 
                new ReportService(provider.GetRequiredService<IUnitOfWork>(), webRootPath));

            return services;
        }
    }
}
