# ToyStore Management

Hệ thống quản lý cửa hàng đồ chơi được xây dựng bằng ASP.NET Core Web API theo mô hình Clean Architecture.

## Technologies

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* Oracle Database 21c
* ASP.NET Core Identity
* JWT Authentication
* SignalR
* MinIO (S3 Compatible Storage)
* AutoMapper
* FluentValidation
* Swagger

## Features

### Authentication & Authorization

* Đăng ký / đăng nhập
* JWT Authentication
* Refresh Token
* Google Login
* Role-based Authorization

### Product Management

* Quản lý sản phẩm
* Quản lý danh mục
* Upload hình ảnh sản phẩm lên MinIO

### Order Management

* Tạo đơn hàng
* Theo dõi trạng thái đơn hàng
* Cập nhật trạng thái theo thời gian thực bằng SignalR

### Reporting

* Báo cáo tồn kho
* Báo cáo doanh thu

## Project Structure

```text
src
├── ToyStoreManagement.API
├── ToyStoreManagement.Application
├── ToyStoreManagement.Domain
└── ToyStoreManagement.Infrastructure

tests
└── ToyStoreManagement.IntegrationTests
```

## Getting Started

### 1. Configure Database

Cập nhật Connection String trong `appsettings.json`.

### 2. Apply Migrations

```bash
dotnet ef database update \
--project src/ToyStoreManagement.Infrastructure \
--startup-project src/ToyStoreManagement.API
```

### 3. Configure MinIO

Cập nhật thông tin MinIO trong `appsettings.json`.

### 4. Run Application

```bash
dotnet run --project src/ToyStoreManagement.API
```

## API Documentation

Sau khi chạy ứng dụng:

```text
https://localhost:5001/swagger
```

## Future Improvements

* Redis Cache
* Docker Deployment
* CI/CD Pipeline
* Unit Tests
