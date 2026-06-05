# 100 CÂU HỎI & TRẢ LỜI VỀ DỰ ÁN TOYSTOREMANAGEMENT

Tài liệu này tổng hợp 100 câu hỏi và câu trả lời quan trọng giúp bạn nhanh chóng nắm vững kiến trúc, công nghệ và logic nghiệp vụ của dự án ToyStoreManagement.

---

## PHẦN 1: TỔNG QUAN & CÔNG NGHỆ (1-10)

1. **Câu hỏi: Dự án ToyStoreManagement là gì?**
   - **Trả lời:** Là hệ thống quản lý cửa hàng đồ chơi toàn diện, hỗ trợ quản lý kho, bán hàng, khách hàng, báo cáo và các tính năng thương mại điện tử.

2. **Câu hỏi: Framework chính được sử dụng trong dự án là gì?**
   - **Trả lời:** Dự án sử dụng .NET 10.0 (ASP.NET Core Web API).

3. **Câu hỏi: Hệ quản trị cơ sở dữ liệu nào được sử dụng?**
   - **Trả lời:** Oracle Database (được cấu hình qua Entity Framework Core).

4. **Câu hỏi: Kiến trúc phần mềm của dự án là gì?**
   - **Trả lời:** Clean Architecture (hay Onion Architecture) với 4 lớp chính: Domain, Application, Infrastructure và API.

5. **Câu hỏi: Các ngôn ngữ lập trình chính được sử dụng?**
   - **Trả lời:** C# 13 là ngôn ngữ chính cho Backend.

6. **Câu hỏi: Dự án sử dụng thư viện nào để ánh xạ (mapping) dữ liệu?**
   - **Trả lời:** AutoMapper.

7. **Câu hỏi: Dự án sử dụng thư viện nào để kiểm tra tính hợp lệ của dữ liệu (validation)?**
   - **Trả lời:** FluentValidation (tích hợp Auto-validation).

8. **Câu hỏi: Hệ thống lưu trữ hình ảnh sản phẩm ở đâu?**
   - **Trả lời:** Sử dụng MinIO (Object Storage) thay vì lưu trực tiếp trong Database hay thư mục local.

9. **Câu hỏi: Cơ chế bảo mật chính của API là gì?**
   - **Trả lời:** JWT (JSON Web Token) Authentication và Role-based Authorization.

10. **Câu hỏi: Dự án sử dụng thư viện nào để xuất báo cáo Excel?**
    - **Trả lời:** EPPlus (OfficeOpenXml).

---

## PHẦN 2: KIẾN TRÚC CLEAN ARCHITECTURE (11-25)

11. **Câu hỏi: Lớp Domain có nhiệm vụ gì?**
    - **Trả lời:** Chứa các thực thể (Entities), Enums, Constants và các Interface cốt lõi. Đây là lớp trung tâm, không phụ thuộc vào bất kỳ lớp nào khác.

12. **Câu hỏi: Lớp Application chứa những gì?**
    - **Trả lời:** Chứa logic nghiệp vụ (Services), DTOs, Interfaces cho các Service, Mappings (AutoMapper) và Validators (FluentValidation).

13. **Câu hỏi: Lớp Infrastructure thực hiện nhiệm vụ gì?**
    - **Trả lời:** Triển khai các Interface từ Domain/Application, bao gồm truy cập dữ liệu (EF Core), Migrations, và các dịch vụ ngoại vi (Email, Storage, Report).

14. **Câu hỏi: Lớp API đóng vai trò gì?**
    - **Trả lời:** Là điểm vào của hệ thống, chứa các Controllers, Middleware, Hubs (SignalR) và cấu hình Dependency Injection.

15. **Câu hỏi: Tại sao lại sử dụng Clean Architecture?**
    - **Trả lời:** Để tách biệt logic nghiệp vụ khỏi UI và Database, giúp hệ thống dễ bảo trì, dễ kiểm thử và dễ mở rộng.

16. **Câu hỏi: Dependency Injection (DI) được đăng ký ở đâu?**
    - **Trả lời:** Các lớp Application và Infrastructure có các phương thức mở rộng `AddApplication` và `AddInfrastructure` được gọi trong `Program.cs` của lớp API.

17. **Câu hỏi: Interface IRepository<T> nằm ở lớp nào?**
    - **Trả lời:** Nằm ở lớp Domain (Interfaces).

18. **Câu hỏi: Lớp nào triển khai IRepository<T>?**
    - **Trả lời:** Lớp Infrastructure (Persistence/Repository.cs).

19. **Câu hỏi: Unit of Work Pattern được sử dụng để làm gì?**
    - **Trả lời:** Để quản lý các transaction, đảm bảo nhiều thao tác dữ liệu được thực hiện đồng nhất (tất cả thành công hoặc tất cả thất bại).

20. **Câu hỏi: DTO (Data Transfer Object) dùng để làm gì?**
    - **Trả lời:** Để vận chuyển dữ liệu giữa các lớp và ẩn đi cấu thực thực thể bên trong Database khi trả về cho Client.

21. **Câu hỏi: Tại sao không nên dùng trực tiếp Entity trong Controller?**
    - **Trả lời:** Để bảo mật (tránh lộ thông tin nhạy cảm), tránh lỗi tham chiếu vòng (circular reference) và giúp API linh hoạt hơn.

22. **Câu hỏi: Lớp nào chịu trách nhiệm thực thi các truy vấn SQL thông qua EF Core?**
    - **Trả lời:** Lớp Infrastructure.

23. **Câu hỏi: "Thin Controller" trong dự án có nghĩa là gì?**
    - **Trả lời:** Controller chỉ nhận request, gọi Service tương ứng và trả về kết quả, không chứa logic nghiệp vụ phức tạp.

24. **Câu hỏi: Các hằng số (Constants) như OrderStatuses nằm ở đâu?**
    - **Trả lời:** Nằm ở lớp Domain (Constants).

25. **Câu hỏi: Tại sao Migration lại nằm ở lớp Infrastructure?**
    - **Trả lời:** Vì Migration liên quan trực tiếp đến việc triển khai Database (Hạ tầng).

---

## PHẦN 3: DOMAIN LAYER - THỰC THỂ & LOGIC LÕI (26-35)

26. **Câu hỏi: Các thực thể chính trong hệ thống gồm những gì?**
    - **Trả lời:** Product, Category, Customer, Order, OrderDetail, InventoryReceipt, InventoryReceiptDetail, AuditLog, v.v.

27. **Câu hỏi: Thực thể Product liên kết với thực thể nào qua khóa ngoại?**
    - **Trả lời:** Category (CategoryId).

28. **Câu hỏi: Thực thể Order và OrderDetail có quan hệ gì?**
    - **Trả lời:** Quan hệ 1-N (Một đơn hàng có nhiều chi tiết đơn hàng).

29. **Câu hỏi: Mục đích của thực thể AuditLog là gì?**
    - **Trả lời:** Lưu lại lịch sử các thao tác thay đổi dữ liệu (Thêm, Sửa, Xóa) trong hệ thống.

30. **Câu hỏi: Thực thể InventoryReceipt dùng để làm gì?**
    - **Trả lời:** Quản lý việc nhập kho sản phẩm từ nhà cung cấp.

31. **Câu hỏi: Kiểu dữ liệu nào được sử dụng cho ID của các thực thể?**
    - **Trả lời:** Thường là Guid (UniqueId).

32. **Câu hỏi: Thực thể DiscountCode dùng để làm gì?**
    - **Trả lời:** Lưu trữ các mã giảm giá áp dụng cho đơn hàng.

33. **Câu hỏi: Quan hệ giữa Product và ProductImage là gì?**
    - **Trả lời:** Quan hệ 1-N (Một sản phẩm có thể có nhiều hình ảnh).

34. **Câu hỏi: Thực thể Wishlist dùng để làm gì?**
    - **Trả lời:** Cho phép khách hàng lưu lại các sản phẩm yêu thích.

35. **Câu hỏi: AppEnums trong Domain Layer chứa những gì?**
    - **Trả lời:** Chứa các định nghĩa kiểu liệt kê như loại người dùng, trạng thái đơn hàng, v.v.

---

## PHẦN 4: APPLICATION LAYER - LOGIC NGHIỆP VỤ (36-50)

36. **Câu hỏi: IProductService cung cấp những chức năng gì?**
    - **Trả lời:** Tìm kiếm, lọc, xem chi tiết, thêm, sửa, xóa sản phẩm và quản lý tồn kho thấp.

37. **Câu hỏi: ProductService sử dụng IMemoryCache để làm gì?**
    - **Trả lời:** Để lưu tạm kết quả tìm kiếm sản phẩm, giúp tăng tốc độ phản hồi và giảm tải cho Database.

38. **Câu hỏi: Làm thế nào để loại bỏ dấu tiếng Việt khi tìm kiếm sản phẩm?**
    - **Trả lời:** Sử dụng `StringHelper.Unaccent` để tạo trường `SearchName` trong Product.

39. **Câu hỏi: ICartService quản lý những gì?**
    - **Trả lời:** Quản lý giỏ hàng của người dùng (Thêm/Xóa/Cập nhật số lượng sản phẩm).

40. **Câu hỏi: IReportService hỗ trợ xuất những loại báo cáo nào?**
    - **Trả lời:** Báo cáo sản phẩm (Excel), báo cáo tồn kho, báo cáo doanh thu.

41. **Câu hỏi: MappingProfile trong lớp Application dùng để cấu hình gì?**
    - **Trả lời:** Cấu hình quy tắc ánh xạ giữa các Entity và DTO bằng AutoMapper.

42. **Câu hỏi: FluentValidation hoạt động như thế nào trong dự án?**
    - **Trả lời:** Các Validator (như `ProductCreateDtoValidator`) định nghĩa các quy tắc ràng buộc dữ liệu đầu vào.

43. **Câu hỏi: Tại sao ProductService lại gọi UnitOfWork.BeginTransactionAsync?**
    - **Trả lời:** Để đảm bảo tính toàn vẹn khi thực hiện các thao tác hàng loạt (bulk operations) như tạo sản phẩm kèm danh mục.

44. **Câu hỏi: IStorageService có nhiệm vụ gì?**
    - **Trả lời:** Định nghĩa các phương thức Upload/Download/Delete file trên hệ thống lưu trữ (MinIO).

45. **Câu hỏi: IEmailService được sử dụng khi nào?**
    - **Trả lời:** Gửi email xác nhận đơn hàng, reset mật khẩu hoặc thông báo khuyến mãi.

46. **Câu hỏi: Làm thế nào để lấy URL của hình ảnh sản phẩm từ MinIO?**
    - **Trả lời:** Thông qua phương thức `GetFileUrl` của IStorageService.

47. **Câu hỏi: Logic tính toán giá sau khi áp dụng mã giảm giá nên nằm ở đâu?**
    - **Trả lời:** Trong `DiscountService` hoặc trực tiếp trong logic của `SaleService`.

48. **Câu hỏi: Chức năng "Low Stock Warning" hoạt động như thế nào?**
    - **Trả lời:** Hệ thống truy vấn các sản phẩm có `StockQuantity` nhỏ hơn một ngưỡng (threshold) nhất định.

49. **Câu hỏi: IAuthService xử lý những công việc gì?**
    - **Trả lời:** Đăng ký, đăng nhập, tạo JWT Token và quản lý User/Role.

50. **Câu hỏi: Tại sao lại dùng `AsNoTracking()` trong các truy vấn lấy dữ liệu?**
    - **Trả lời:** Để tăng hiệu suất khi chỉ cần đọc dữ liệu mà không có ý định cập nhật chúng.

---

## PHẦN 5: INFRASTRUCTURE LAYER - HẠ TẦNG (51-65)

51. **Câu hỏi: AppDbContext kế thừa từ lớp nào?**
    - **Trả lời:** IdentityDbContext (để tích hợp sẵn ASP.NET Core Identity).

52. **Câu hỏi: Làm thế nào để cấu hình kiểu dữ liệu DECIMAL cho Oracle trong EF Core?**
    - **Trả lời:** Sử dụng `property.SetColumnType("NUMBER(18, 2)")` trong `OnModelCreating`.

53. **Câu hỏi: Dự án xử lý kiểu BOOLEAN trong Oracle như thế nào?**
    - **Trả lời:** Chuyển đổi thành kiểu `NUMBER(1)` (0 là false, 1 là true).

54. **Câu hỏi: Cơ chế tự động lưu Audit Log trong AppDbContext hoạt động như thế nào?**
    - **Trả lời:** Ghi đè phương thức `SaveChangesAsync`, duyệt qua các thay đổi trong `ChangeTracker` và lưu vào bảng `AuditLogs`.

55. **Câu hỏi: MinioStorageService triển khai Interface nào?**
    - **Trả lời:** IStorageService.

56. **Câu hỏi: Thông tin kết nối MinIO được lấy từ đâu?**
    - **Trả lời:** Từ `appsettings.json` thông qua `MinioSettings` DTO.

57. **Câu hỏi: ReportService sử dụng thư viện nào để đọc template Excel?**
    - **Trả lời:** EPPlus.

58. **Câu hỏi: Làm thế nào để Repository có thể dùng chung (Generic) cho mọi Entity?**
    - **Trả lời:** Sử dụng `Repository<TEntity>` triển khai `IRepository<TEntity>`.

59. **Câu hỏi: UnitOfWork quản lý DbContext như thế nào?**
    - **Trả lời:** Nó giữ một instance duy nhất của DbContext để chia sẻ giữa các Repository trong cùng một phạm vi yêu cầu (Scoped).

60. **Câu hỏi: Migration trong Oracle có gì khác so với SQL Server?**
    - **Trả lời:** Cần chú ý đến tên bảng/cột (thường in hoa) và các ràng buộc về schema.

61. **Câu hỏi: Tại sao phải sử dụng `SetSchema("C##ORACLEDB2026")`?**
    - **Trả lời:** Để đảm bảo các bảng được tạo đúng schema mong muốn trong Oracle.

62. **Câu hỏi: EmailService sử dụng giao thức nào để gửi mail?**
    - **Trả lời:** Thường là SMTP (Sử dụng MailKit hoặc SmtpClient).

63. **Câu hỏi: Làm thế nào để cấu hình bảng Identity (User, Role) in hoa trong Oracle?**
    - **Trả lời:** Sử dụng `entity.SetTableName(tableName.ToUpper())` trong `OnModelCreating`.

64. **Câu hỏi: Infrastructure Layer có phụ thuộc vào API Layer không?**
    - **Trả lời:** Không, nó chỉ phụ thuộc vào Domain và Application.

65. **Câu hỏi: Lớp Persistence/UnitOfWork.cs thực hiện việc gì khi gọi `SaveChangesAsync`?**
    - **Trả lời:** Gọi trực tiếp `_context.SaveChangesAsync()` để commit toàn bộ thay đổi vào DB.

---

## PHẦN 6: WEB API LAYER - ĐIỂM TRUY CẬP (66-80)

66. **Câu hỏi: ExceptionMiddleware dùng để làm gì?**
    - **Trả lời:** Để bắt tất cả các lỗi ngoại lệ chưa được xử lý và trả về một định dạng JSON thống nhất cho Client.

67. **Câu hỏi: BaseCrudController cung cấp những gì?**
    - **Trả lời:** Các phương thức cơ bản như Get, Post, Put, Delete giúp giảm lặp mã cho các Controller con.

68. **Câu hỏi: NotificationHub (SignalR) được sử dụng để làm gì?**
    - **Trả lời:** Để gửi thông báo thời gian thực (real-time) cho người dùng khi có đơn hàng mới hoặc cập nhật kho.

69. **Câu hỏi: Làm thế nào để cấu hình JWT trong API?**
    - **Trả lời:** Sử dụng `AddAuthentication` và `AddJwtBearer` trong `Program.cs`.

70. **Câu hỏi: Swagger dùng để làm gì?**
    - **Trả lời:** Tự động tạo tài liệu hướng dẫn sử dụng API và cho phép test API trực tiếp trên trình duyệt.

71. **Câu hỏi: AuthController xử lý các endpoint nào?**
    - **Trả lời:** `/api/auth/login`, `/api/auth/register`, `/api/auth/refresh-token`, v.v.

72. **Câu hỏi: Làm thế nào để giới hạn quyền truy cập vào một Controller?**
    - **Trả lời:** Sử dụng thuộc tính `[Authorize(Roles = "Admin")]`.

73. **Câu hỏi: CartController lấy thông tin giỏ hàng dựa trên cái gì?**
    - **Trả lời:** Thường dựa trên UserId được trích xuất từ JWT Token của người dùng đang đăng nhập.

74. **Câu hỏi: InventoryController quản lý các hoạt động nào?**
    - **Trả lời:** Quản lý nhập kho (InventoryReceipts) và kiểm kê.

75. **Câu hỏi: SalesController quản lý các hoạt động nào?**
    - **Trả lời:** Tạo đơn hàng (Checkout), thanh toán và xem lịch sử đơn hàng.

76. **Câu hỏi: Middleware nào được chạy đầu tiên trong Pipeline?**
    - **Trả lời:** Thường là ExceptionMiddleware để đảm bảo bắt được lỗi từ mọi Middleware phía sau.

77. **Câu hỏi: Tại sao nên sử dụng `[ApiController]` attribute?**
    - **Trả lời:** Để kích hoạt các tính năng tự động như Model Validation, Inferring parameter sources, v.v.

78. **Câu hỏi: Cors Policy được cấu hình để làm gì?**
    - **Trả lời:** Cho phép các ứng dụng Frontend (React/Angular) từ domain khác có thể gọi API.

79. **Câu hỏi: Làm thế nào để upload file thông qua API?**
    - **Trả lời:** Sử dụng `IFormFile` trong tham số của Action.

80. **Câu hỏi: Thư mục `wwwroot/templates` chứa những gì?**
    - **Trả lời:** Chứa các file mẫu (Excel, Word) dùng cho việc xuất báo cáo hoặc in ấn.

---

## PHẦN 7: BẢO MẬT & IDENTITY (81-90)

81. **Câu hỏi: IdentityUser là gì?**
    - **Trả lời:** Là lớp cơ sở của ASP.NET Core Identity chứa thông tin người dùng như Username, Email, PasswordHash.

82. **Câu hỏi: Làm thế nào để hash mật khẩu người dùng?**
    - **Trả lời:** Sử dụng `UserManager<IdentityUser>` cung cấp sẵn các cơ chế bảo mật để hash mật khẩu.

83. **Câu hỏi: JWT gồm có 3 phần nào?**
    - **Trả lời:** Header, Payload (chứa Claims) và Signature.

84. **Câu hỏi: Claim trong JWT là gì?**
    - **Trả lời:** Là các mảnh thông tin về người dùng (như ID, Name, Role) được mã hóa trong Token.

85. **Câu hỏi: Tại sao cần Refresh Token?**
    - **Trả lời:** Để cấp mới Access Token khi nó hết hạn mà không bắt người dùng phải đăng nhập lại.

86. **Câu hỏi: Làm thế nào để bảo mật chuỗi Connection String và JWT Secret?**
    - **Trả lời:** Sử dụng Environment Variables hoặc `appsettings.json` (không nên commit file này nếu chứa thông tin thật).

87. **Câu hỏi: Role-based Authorization hoạt động như thế nào?**
    - **Trả lời:** Kiểm tra xem vai trò (Role) trong JWT của người dùng có khớp với vai trò được yêu cầu tại Controller/Action hay không.

88. **Câu hỏi: `[AllowAnonymous]` dùng để làm gì?**
    - **Trả lời:** Cho phép truy cập vào API mà không cần đăng nhập (ví dụ: xem danh sách sản phẩm).

89. **Câu hỏi: Làm thế nào để lấy thông tin UserId của người dùng hiện tại trong Controller?**
    - **Trả lời:** Qua `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`.

90. **Câu hỏi: Password Policy trong Identity có thể cấu hình những gì?**
    - **Trả lời:** Độ dài tối thiểu, yêu cầu chữ hoa, chữ thường, số, ký tự đặc biệt.

---

## PHẦN 8: TESTING & VALIDATION (91-95)

91. **Câu hỏi: Dự án sử dụng framework kiểm thử nào?**
    - **Trả lời:** xUnit.

92. **Câu hỏi: Integration Test khác Unit Test ở điểm nào?**
    - **Trả lời:** Integration Test kiểm tra sự phối hợp giữa nhiều thành phần (bao gồm cả Database thật), còn Unit Test kiểm tra một hàm/lớp đơn lẻ một cách cô lập.

93. **Câu hỏi: Lớp `ProductIntegrationTests` kiểm tra những gì?**
    - **Trả lời:** Kiểm tra các luồng nghiệp vụ thực tế của sản phẩm từ lúc gửi request API đến khi dữ liệu được lưu vào DB.

94. **Câu hỏi: FluentValidation tự động trả về lỗi gì khi dữ liệu không hợp lệ?**
    - **Trả lời:** Trả về lỗi 400 Bad Request kèm theo danh sách chi tiết các trường bị lỗi.

95. **Câu hỏi: Làm thế nào để mock (giả lập) dữ liệu trong Unit Test?**
    - **Trả lời:** Sử dụng thư viện như Moq hoặc NSubstitute.

---

## PHẦN 9: TRIỂN KHAI & TÍNH NĂNG NÂNG CAO (96-100)

96. **Câu hỏi: Làm thế nào để chạy dự án lần đầu tiên?**
    - **Trả lời:** Cấu hình Connection String Oracle -> Chạy `Update-Database` -> Nhấn F5 trong Visual Studio.

97. **Câu hỏi: Tại sao dự án cần trường `SearchName` (không dấu) trong bảng Product?**
    - **Trả lời:** Vì Oracle (và nhiều DB khác) tìm kiếm tiếng Việt có dấu khá phức tạp, việc dùng SearchName giúp tìm kiếm nhanh và chính xác hơn.

98. **Câu hỏi: Hệ thống xử lý thế nào khi xóa một danh mục (Category) đang có sản phẩm?**
    - **Trả lời:** Thường sẽ có ràng buộc khóa ngoại ngăn cản việc này hoặc Service sẽ kiểm tra và báo lỗi.

99. **Câu hỏi: Làm thế nào để mở rộng hệ thống thêm tính năng "Đánh giá sản phẩm" (Product Review)?**
    - **Trả lời:** Tạo Entity `ProductReview` -> Cấu hình DbContext -> Tạo `ReviewService` và `ProductReviewsController`.

100. **Câu hỏi: Làm thế nào để quản lý các Store Settings (Cấu hình cửa hàng)?**
    - **Trả lời:** Sử dụng thực thể `StoreSetting` để lưu các key-value như tên cửa hàng, địa chỉ, hotline, phí ship.

---
**Chúc bạn nắm vững và phát triển dự án thành công!**
