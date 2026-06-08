using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.API.Hubs;
using ToyStoreManagement.API.Middleware;
using ToyStoreManagement.Application;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.API.Services;
using ToyStoreManagement.Infrastructure;

using ToyStoreManagement.Infrastructure.Persistence;
using ToyStoreManagement.Domain.Constants;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.WriteTo.Console()
                 .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

// Cấu hình JWT Authentication
// ... (Jwt configuration)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Đăng ký Memory Cache
builder.Services.AddMemoryCache();

// Đăng ký Notification Service
builder.Services.AddScoped<INotificationService, NotificationService>();

// Cấu hình Hangfire
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());
builder.Services.AddHangfireServer();

// Cấu hình SignalR
builder.Services.AddSignalR();

// Đăng ký các dịch vụ qua Extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot"));

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<ToyStoreManagement.Application.Mappings.MappingProfile>();
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ToyStoreManagement.Application.Validators.ProductCreateDtoValidator>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v1",
        Title = "Toy Store Management API",
        Description = "Hệ thống quản lý cửa hàng đồ chơi"
    });
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseCors();
app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json","ToyStore API V1");
        options.RoutePrefix = string.Empty; // Swagger chạy tại root
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");

// --- DATA BACKFILL FOR SEARCH ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Seed Roles and Admin
    await DbInitializer.SeedRolesAndAdminAsync(services);

    var context = services.GetRequiredService<ToyStoreManagement.Infrastructure.Persistence.AppDbContext>();
    var productsToUpdate = await context.Products
        .Where(p => p.SearchName == null)
        .ToListAsync();

    if (productsToUpdate.Any())
    {
        foreach (var product in productsToUpdate)
        {
            product.SearchName = ToyStoreManagement.Application.Helpers.StringHelper.Unaccent(product.Name);
        }
        await context.SaveChangesAsync();
    }
}

app.Run();

public partial class Program { }