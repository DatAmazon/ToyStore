 🧸 ToyStore Management System
  Enterprise-Grade E-Commerce & Inventory Solution : .NET 10, Oracle Database, Clean Architecture, Minio Storage

  🌟 Tổng Quan Dự Án
  ToyStore Management là hệ thống quản trị thương mại điện tử toàn diện

  🏗️ Kiến Trúc Hệ Thống (Clean Architecture)
  Dự án được phân tách thành các lớp (Layers) nghiêm ngặt để đảm bảo nguyên lý Separation of Concerns:

   * Core Domain: Chứa các Business Entities, định nghĩa quy tắc nghiệp vụ cốt lõi, hoàn toàn độc lập với các thư viện
     bên ngoài.
   * Application Logic: Điều phối luồng dữ liệu thông qua các Use Cases, DTOs, và tích hợp các công cụ như AutoMapper,
     FluentValidation.
   * Infrastructure: Tầng hạ tầng xử lý các tác vụ "nặng" như:
       * Oracle EF Core Provider: Tối ưu hóa truy vấn và quản lý Transaction trên Oracle.
       * Storage Service: Tích hợp Minio (S3 Compatible) để quản lý tài nguyên số.
       * Security: Triển khai Identity Framework & JWT Authentication.
   * API Layer: Cung cấp RESTful Endpoints chuẩn hóa, tích hợp Global Exception Middleware và Swagger Documentation.

  ---

  🚀 Tính Năng Kỹ Thuật Nổi Bật

  💎 Hệ Quản Trị Dữ Liệu Enterprise
   - Oracle Code First Migrations: Quản lý cấu trúc Database phức tạp (Tablespace, Schema) hoàn toàn bằng mã nguồn.
   - Audit Logs System: Tự động theo dõi và ghi lại mọi biến động dữ liệu nhạy cảm, đảm bảo tính minh bạch cho quy trình
     quản trị.

  🛡️ Bảo Mật Đa Tầng
   - Authentication: Kết hợp Identity Core, JWT cho Mobile/Web và Google OAuth 2.0 cho khách hàng.
   - Authorization: Phân quyền dựa trên Role (RBAC) và Policy-based, kiểm soát truy cập đến từng API Endpoint.

  ⚡ Hiệu Năng & Trải Nghiệm
   - Real-time Engine: Sử dụng SignalR để đồng bộ trạng thái đơn hàng và thông báo tức thời giữa hệ thống và người dùng.
   - Modern Storage: Áp dụng tư duy Cloud-native bằng cách tách biệt việc lưu trữ file vật lý sang Minio S3, giúp giảm
     tải dung lượng Database.
   - Scalable Reporting: Hệ thống xuất báo cáo (Inventory/Sales) linh hoạt dựa trên Template Engine chuyên nghiệp.


  📂 Cấu Trúc Thư Mục Chính

   1 ToyStoreManagement/
   2 ├── src/
   3 │   ├── ToyStoreManagement.API            # RESTful API & Hubs
   4 │   ├── ToyStoreManagement.Application    # Use Cases & Interfaces
   5 │   ├── ToyStoreManagement.Domain         # Entities & Core Logic
   6 │   └── ToyStoreManagement.Infrastructure # Oracle DB, S3, Email Services
   7 └── tests/
   8     └── ToyStoreManagement.IntegrationTests # Automated API Tests

  ---

  ⚙️ Hướng Dẫn Cài Đặt (Quick Start)

   1. Cấu hình Database: Cập nhật Connection String Oracle của bạn trong appsettings.json.
   2. Khởi tạo Database:

   1     dotnet ef database update --project ToyStoreManagement.Infrastructure --startup-project ToyStoreManagement.API
   3. Cấu hình Minio: Đảm bảo Server Minio đang chạy để hệ thống có thể upload hình ảnh sản phẩm.
   4. Chạy dự án:
   1     dotnet run --project ToyStoreManagement.API
