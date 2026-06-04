# TÀI LIỆU HƯỚNG DẪN & ĐÁNH GIÁ DỰ ÁN TOYSTOREMANAGEMENT

## 1. TỔNG QUAN DỰ ÁN
Dự án **ToyStoreManagement** là một hệ thống quản lý cửa hàng đồ chơi toàn diện, được xây dựng trên nền tảng **.NET 10**. Mục tiêu của dự án là cung cấp các giải pháp quản lý kho, bán hàng, khách hàng và báo cáo hiệu quả cho các cửa hàng đồ chơi quy mô vừa và lớn.

---

## 2. KIẾN TRÚC & CÔNG NGHỆ (TECH STACK)
Hệ thống tuân thủ nghiêm ngặt kiến trúc **Clean Architecture (Onion Architecture)**, giúp tách biệt rõ ràng logic nghiệp vụ khỏi các yếu tố hạ tầng.

### Công nghệ lõi:
- **Ngôn ngữ:** C# 13 / .NET 10.
- **Cơ sở dữ liệu:** Oracle Database (EF Core Provider).
- **Bảo mật:** ASP.NET Core Identity & JWT (JSON Web Token).
- **Validation:** FluentValidation (Auto-validation).
- **Mapping:** AutoMapper.
- **Báo cáo:** EPPlus (Excel) và hỗ trợ PDF.
- **Kiểm thử:** xUnit & Integration Testing.

---

## 3. CÁC ĐIỂM MẠNH (STRENGTHS)

### ✅ Kiến trúc Clean Architecture chuẩn mực
Dự án được phân chia thành 4 lớp rõ rệt (`Domain`, `Application`, `Infrastructure`, `API`). Điều này giúp:
- **Dễ bảo trì:** Thay đổi cơ sở dữ liệu hoặc framework UI không ảnh hưởng đến logic nghiệp vụ.
- **Dễ mở rộng:** Có thể thêm các tính năng mới mà không phá vỡ cấu trúc hiện tại.

### ✅ Triển khai Design Patterns chuyên nghiệp
- **Unit of Work & Generic Repository:** Quản lý giao dịch (Transaction) tập trung, đảm bảo tính toàn vẹn dữ liệu khi thực hiện nhiều thao tác ghi đồng thời.
- **Dependency Injection (DI):** Giảm sự phụ thuộc giữa các thành phần, giúp mã nguồn linh hoạt và dễ kiểm thử hơn.

### ✅ Bảo mật và Phân quyền (Security)
- Tích hợp **ASP.NET Core Identity** cho quản lý người dùng và vai trò.
- Cơ chế **JWT Authentication** giúp bảo mật các API endpoint và hỗ trợ tốt cho các ứng dụng Client-side (React/Angular).

### ✅ Xử lý lỗi và Kiểm soát dữ liệu tập trung
- **Global Exception Middleware:** Bắt mọi lỗi chưa được xử lý và trả về phản hồi đồng nhất (ErrorResponse), giúp giao diện người dùng dễ dàng xử lý lỗi.
- **FluentValidation:** Tự động kiểm tra tính hợp lệ của dữ liệu đầu vào ngay tại lớp Application, ngăn chặn dữ liệu "rác" vào hệ thống.

### ✅ Khả năng kiểm thử (Testability)
- Dự án có sẵn lớp **Integration Tests** chạy trên DB thật, giúp đảm bảo logic nghiệp vụ hoạt động chính xác trong môi trường thực tế.

---

## 4. CÁC ĐIỂM CHƯA ĐƯỢC (WEAKNESSES / LIMITATIONS)

### ❌ Thiếu cơ chế Caching (Lưu trữ đệm)
Hiện tại, mọi yêu cầu lấy dữ liệu (như danh sách sản phẩm, danh mục) đều truy vấn trực tiếp vào Oracle Database.
- **Hệ quả:** Có thể gây quá tải database khi số lượng người dùng tăng cao.
- **Đề xuất:** Tích hợp **Redis** hoặc **MemoryCache** cho các dữ liệu ít thay đổi.

### ❌ Quản lý tệp tin (File Storage) còn hạn chế
Các hình ảnh sản phẩm đang được lưu trực tiếp trong thư mục `wwwroot` của server API.
- **Hệ quả:** Khó mở rộng hệ thống (Scalability). Nếu chạy nhiều instance API, các file sẽ không đồng bộ.
- **Đề xuất:** Chuyển sang sử dụng Cloud Storage như **Azure Blob Storage** hoặc **AWS S3**.

### ❌ Thiếu hệ thống Logging & Audit Trail chi tiết
Mặc dù đã có ghi lỗi trong Middleware, nhưng hệ thống thiếu:
- **Audit Logs:** Theo dõi ai đã thay đổi số lượng kho, ai đã xóa đơn hàng,...
- **Structured Logging:** Thiếu việc tích hợp các công cụ như **Serilog** để đẩy log lên ELK Stack hoặc Seq.

### ❌ Kiểm soát tranh chấp dữ liệu (Concurrency)
Hệ thống chưa triển khai **Optimistic Concurrency** (RowVersion).
- **Hệ quả:** Nếu hai nhân viên cùng cập nhật số lượng kho của một sản phẩm tại cùng một thời điểm, dữ liệu của người cập nhật sau có thể ghi đè lên người trước mà không có cảnh báo.

### ❌ Mô hình Domain còn "gầy" (Anemic Domain Model)
Logic nghiệp vụ chủ yếu nằm ở các lớp Service (`Application`). Các thực thể (Entities) chỉ chứa thuộc tính.
- **Đề xuất:** Di chuyển các logic nghiệp vụ lõi (như tính toán giá sau giảm giá, validate trạng thái đơn hàng) vào chính các Entity trong lớp Domain.

### ❌ Thiếu cấu hình CI/CD và Docker
Dự án chưa có file `Dockerfile` hay các script cấu hình triển khai tự động.
- **Hệ quả:** Việc triển khai lên môi trường Production còn thủ công và dễ xảy ra sai sót.

---

## 5. ĐỀ XUẤT CẢI TIẾN TRONG TƯƠNG LAI
1.  **Microservices Readiness:** Cân nhắc tách module Sales và Inventory nếu quy mô dự án mở rộng.
2.  **Health Checks:** Thêm endpoint kiểm tra sức khỏe hệ thống (Database, Disk space).
3.  **Rate Limiting:** Ngăn chặn các cuộc tấn công Brute-force hoặc Spam API.
4.  **API Versioning:** Quản lý các phiên bản API (v1, v2) để tránh làm hỏng các client cũ khi cập nhật.

---

## 6. HƯỚNG DẪN CÀI ĐẶT NHANH
1.  **Database:** Cấu hình connection string Oracle trong `appsettings.json`.
2.  **Migrations:** Chạy `Update-Database` để tạo cấu trúc bảng.
3.  **Run:** Mở `ToyStoreManagement.slnx` bằng Visual Studio 2022/2025 và nhấn F5.
4.  **Swagger:** Truy cập `https://localhost:xxxx/index.html` để xem tài liệu API.
