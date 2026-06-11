 🏗️ Kiến Trúc Hệ Thống (Architectural Overview)

  Dự án áp dụng Clean Architecture (Onion Architecture) giúp tách biệt rõ ràng các lớp logic, đảm bảo tính dễ bảo trì
  (maintainability) và dễ mở rộng (scalability):

   - ToyStoreManagement.Domain: Trái tim của hệ thống. Chứa các Entity, Enums và Interfaces cốt lõi. Không phụ thuộc vào
     bất kỳ thư viện bên ngoài nào.
   - ToyStoreManagement.Application: Chứa logic nghiệp vụ (Use Cases), DTOs, Mapping (AutoMapper) và Validation
     (FluentValidation).
   - ToyStoreManagement.Infrastructure: Triển khai các dịch vụ bên ngoài: Database (EF Core), Storage (Minio), Email
     Service, Security.
   - ToyStoreManagement.API: Lớp giao tiếp với bên ngoài (RESTful API), xử lý Middleware, Authentication/Authorization
     và SignalR Hubs.

  ---

  🚀 Tính Năng Nổi Bật (Key Features)

  👤 Cho Khách Hàng (Customer Experience)
   - E-Commerce Flow: Tìm kiếm sản phẩm thông minh, quản lý giỏ hàng (Cart) và danh sách yêu thích (Wishlist).
   - Checkout System: Quy trình thanh toán linh hoạt (Hỗ trợ Guest Checkout), tích hợp mã giảm giá (Discount Codes).
   - Social Interaction: Đánh giá sản phẩm (Reviews) và phản hồi từ người dùng.

  🔐 Cho Quản Trị Viên (Admin & Management)
   - Smart Dashboard: Thống kê doanh thu, đơn hàng và tình trạng tồn kho theo thời gian thực.
   - Inventory Management: Quản lý nhập kho (Inventory Receipts), nhà cung cấp (Suppliers) và cảnh báo hàng tồn.
   - Advanced Audit Logs: Ghi lại mọi hoạt động thay đổi dữ liệu nhạy cảm để đảm bảo tính an ninh và minh bạch.
   - Dynamic Settings: Cấu hình cửa hàng, tiền tệ và hệ thống linh hoạt qua Store Settings.

  🛠️ Kỹ Thuật Hệ Thống (System Engineering)
   - Real-time Notifications: Sử dụng SignalR để thông báo đơn hàng mới hoặc cập nhật trạng thái kho ngay lập tức.
   - Object Storage: Tích hợp Minio để quản lý hình ảnh sản phẩm chuyên nghiệp thay vì lưu trực tiếp trong server.
   - Reporting Service: Xuất báo cáo (Sales, Inventory, Customer) với hệ thống Template chuyên nghiệp.
   - Robust Security: Identity Framework kết hợp JWT, phân quyền Role-based (Admin, Staff, Customer).

  ---

  🛠️ Công Nghệ Sử Dụng (Tech Stack)

  ┌──────────────────────┬────────────────────────────────────────────────┐
  │ Category             │ Technology                                     │
  ├──────────────────────┼────────────────────────────────────────────────┤
  │ Back-end             │ .NET 8, ASP.NET Core Web API                   │
  │ Database             │ SQL Server, Entity Framework Core (Code First) │
  │ Storage              │ Minio (S3 Compatible Storage)                  │
  │ Real-time            │ SignalR                                        │
  │ Mapping & Validation │ AutoMapper, FluentValidation                   │
  │ Testing              │ xUnit, Integration Tests                       │
  │ Documentation        │ Swagger (OpenAPI)                              │
  └──────────────────────┴────────────────────────────────────────────────┘
  ---

  📈 Sơ Đồ Cơ Sở Dữ Liệu (Database Schema)
  Hệ thống quản lý hơn 15+ bảng dữ liệu được chuẩn hóa, bao gồm:
   - Core: Products, Categories, Suppliers.
   - Sales: Orders, OrderDetails, CartItems, DiscountCodes.
   - System: AuditLogs, StoreSettings, AppUsers.
  ---

  🛡️ Chất Lượng Mã Nguồn (Quality Assurance)
   - Design Patterns: Repository Pattern, Unit of Work, Dependency Injection.
   - Middleware: Xử lý lỗi tập trung (Global Exception Handling) và ghi log hoạt động.
   - Testing: Đã triển khai Integration Tests để đảm bảo các API endpoint hoạt động chính xác trong môi trường thực tế.
  ---

  ⚙️ Hướng Dẫn Cài Đặt (Quick Start)

   1. Clone project:

   1    git clone https://github.com/yourusername/ToyStoreManagement.git
   2. Cấu hình Database:
     Cập nhật ConnectionStrings trong file appsettings.json.
   3. Update Database:
    dotnet ef database update --project ToyStoreManagement.Infrastructure --startup-project ToyStoreManagement.API
   4. Run:

   1    dotnet run --project ToyStoreManagement.API

  ---
