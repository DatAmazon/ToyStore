using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ToyStoreManagement.Data;
using ToyStoreManagement.IRepositories;
using ToyStoreManagement.IServices;
using ToyStoreManagement.Repositories;
using ToyStoreManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Connection String kết nối Oracle DB
// Định dạng Oracle Connection String: User CustomerId=tên_user;Password=mật_khẩu;Data Source=IP_Oracle:Port/Tên_Service_Hoặc_SID
var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

// 2. Đăng ký tầng nghiệp vụ qua Cơ chế Dependency Injection (Scoped Service)
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IRepository, Repository>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// 2. Cấu hình các thông tin hiển thị trên giao diện Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Toy Store Management API",
        Description = "",
        Contact = new OpenApiContact
        {
            Name = "",
            Email = "tech@toystore.com"
        }
    });
});

builder.Services.AddOpenApi();



var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger(); // Sinh ra file json mô tả API
    app.UseSwaggerUI(options =>
    {
        // Định nghĩa đường dẫn file json và tên hiển thị ở góc màn hình
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToyStore API V1");

        // Mẹo nhỏ: Để cấu hình này nếu bạn muốn khi gõ "localhost:port" là nó tự nhảy vào trang Swagger luôn, không cần gõ thêm chữ "/swagger" nữa
        options.RoutePrefix = string.Empty;
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
