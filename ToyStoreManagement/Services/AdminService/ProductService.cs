using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using SkiaSharp;
using Spire.Doc;
using Spire.Doc.Fields.Shapes.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ToyStoreManagement.Data;
using ToyStoreManagement.Entities;

namespace ToyStoreManagement.Services.AdminService
{
    public class ProductsService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. GetAll kèm Include
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        // 2. GetById kèm Include
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // 3. Logic Create đơn lẻ
        public async Task<(bool Success, string Message, Product? Data)> CreateAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) return (false, "Tên không được trống", null);
            if (product.Price < 0) return (false, "Giá không thể âm", null);

            product.ProductId = Guid.NewGuid();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return (true, "Thành công", product);
        }

        // 4. Logic Create Multiple với Auto Category
        public async Task<(bool Success, string Message)> CreateWithAutoCategoryAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any()) return (false, "Danh sách trống");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var categoryIdsInRequest = products.Select(p => p.CategoryId).Distinct().ToList();

                // 2. Tải tất cả các Category hiện có trong DB vào Tracker một lần duy nhất
                var existingCategories = await _context.Categories
                    .Where(c => categoryIdsInRequest.Contains(c.CategoryId))
                    .ToDictionaryAsync(c => c.CategoryId);


                foreach (var product in products)
                {
                    // Kiểm tra xem Category đã có trong DB chưa
                    if (!existingCategories.ContainsKey(product.CategoryId))
                    {
                        // Nếu chưa có, tạo mới và thêm vào Dictionary để sản phẩm sau dùng lại
                        var newCategory = new Category
                        {
                            CategoryId = product.CategoryId != Guid.Empty ? product.CategoryId : Guid.NewGuid(),
                            CategoryName = "Danh mục mới tự động"
                        };

                        await _context.Categories.AddAsync(newCategory);
                        existingCategories.Add(newCategory.CategoryId, newCategory);
                    }

                    // QUAN TRỌNG NHẤT:
                    product.ProductId = Guid.NewGuid();

                    // Ngắt kết nối Object Category kèm theo nếu có để tránh EF cố Insert lại
                    // Chúng ta chỉ dùng CategoryId (Foreign Key) để làm việc
                    product.Category = null;
                }

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Xử lý thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi xử lý hàng loạt: {ex.Message}");
            }
        }

        // 5. Logic Update & Delete
        public async Task<bool> UpdateAsync(Product product)
        {
            var exists = await _context.Products.AnyAsync(p => p.ProductId == product.ProductId);
            if (!exists) return false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> ExportProductsToExcelAsync()
        {
            var products = await _context.Products
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .ToListAsync();

            var dataList = products.Select(p => new
            {
                Name = p.Name,
                Price = p.Price.ToString("N0") + " VNĐ",
                StockQuantity = p.StockQuantity,
                MinimumAge = p.MinimumAge,
                Manufacturer = p.Manufacturer,
                CategoryName = p.Category?.CategoryName ?? "N/A"
            }).ToList();

            var templateData = new { items = dataList };

            string templatePath = Path.Combine(_env.WebRootPath, "templates", "Print", "ProductReport.xlsx");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Không tìm thấy file template tại: " + templatePath);
            }

            using (var memoryStream = new MemoryStream())
            {
                await memoryStream.SaveAsByTemplateAsync(templatePath, templateData);
                return memoryStream.ToArray();
            }
        }

        //public async Task<byte[]> PDFProductReceipt()
        //{
        //    // 1. Lấy dữ liệu (Ví dụ lấy từ Database dựa trên OrderId)
        //    var products = await _context.Products.Include(p => p.Category).AsNoTracking().ToListAsync();

        //    // Tính toán sơ bộ
        //    decimal totalMoney = products.Sum(p => p.Price * p.StockQuantity);

        //    // 2. Gom tất cả data vào một object 'value' khớp với các thẻ {{}} trong Word
        //    var value = new
        //    {
        //        // Thông tin đơn vị & Khách hàng
        //        ReportParameterUnitName = "DAT AMAZON TOY STORE",
        //        ReportParameterAddressName = "123 Đường ABC, Hà Nội",
        //        ReportParameterUserName = "Nguyễn Văn A",
        //        ReportParameterUserYearOld = "30",
        //        ReportParameterGender = "Nam",
        //        ReportParameterUserAddress = "Quận Cầu Giấy, Hà Nội",
        //        ReportParameterPhone = "0987.654.321",
        //        ReportParameterReceptionCode = "PH-123456",
        //        ReportParameterPayment = "Chuyển khoản",

        //        // Danh sách sản phẩm (Bắt buộc tên là 'items')
        //        items = products.Select((p, index) => new {
        //            STT = index + 1,
        //            Name = p.Name,
        //            Price = p.Price.ToString("N0"),
        //            Stock = p.StockQuantity,
        //            Age = p.MinimumAge + "+",
        //            Manufacturer = p.Manufacturer,
        //            Category = p.Category?.CategoryName ?? "N/A"
        //        }).ToList(),

        //        // Tổng hợp & Footer
        //        ReportParameterIndex = products.Count,
        //        ReportParameterIndexString = "Hai",
        //        ReportParameterTotalMoney = totalMoney.ToString("N0") + " VNĐ",
        //        ReportParameterTotalMoneyString = "Một triệu hai trăm nghìn đồng",
        //        ReportParameterCreatedDate = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",
        //        ReportParameterReceipt = "Người nhận A",
        //        ReportParameterCashier = "Thu ngân B",
        //        ReportParameterAccountant = "Kế toán C"
        //    };

        //    // 3. Đường dẫn template
        //    string templatePath = Path.Combine(_env.WebRootPath, "templates", "Print", "ProductPrint.docx");

        //    // 4. Xuất mảng byte
        //    using (var ms = new MemoryStream())
        //    {
        //        // 1. MiniExcel ghi dữ liệu vào stream
        //        await ms.SaveAsByTemplateAsync(templatePath, value);

        //        // CỰC KỲ QUAN TRỌNG: Đưa con trỏ về 0 sau khi MiniExcel ghi xong
        //        ms.Position = 0;

        //        // Kiểm tra xem MiniExcel có thực sự ghi được gì không
        //        if (ms.Length == 0) throw new Exception("Template không hợp lệ hoặc không có dữ liệu!");

        //        // 2. Dùng Spire.Doc để đọc từ stream đã có dữ liệu
        //        Document doc = new Document();
        //        doc.LoadFromStream(ms, FileFormat.Docx);

        //        using (var pdfStream = new MemoryStream())
        //        {
        //            doc.SaveToStream(pdfStream, FileFormat.PDF);
        //            return pdfStream.ToArray();
        //        }
        //    }
        //}



        public async Task<byte[]> PDFProductReceipt()
        {
            // =========================
            // 1. Lấy dữ liệu
            // =========================
            var products = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            decimal totalMoney = products.Sum(p => p.Price * p.StockQuantity);

            // =========================
            // 2. Data object
            // =========================
            var value = new
            {
                ReportParameterUnitName = "DAT AMAZON TOY STORE",
                ReportParameterAddressName = "123 Đường ABC, Hà Nội",
                ReportParameterUserName = "Nguyễn Văn A",
                ReportParameterUserYearOld = "30",
                ReportParameterGender = "Nam",
                ReportParameterUserAddress = "Quận Cầu Giấy, Hà Nội",
                ReportParameterPhone = "0987.654.321",
                ReportParameterReceptionCode = "PH-123456",
                ReportParameterPayment = "Chuyển khoản",

                ReportParameterIndex = products.Count,
                ReportParameterIndexString = "Hai",
                ReportParameterTotalMoney = totalMoney.ToString("N0") + " VNĐ",
                ReportParameterTotalMoneyString = "Một triệu hai trăm nghìn đồng",

                ReportParameterCreatedDate =
                    $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}",

                ReportParameterReceipt = "Người nhận A",
                ReportParameterCashier = "Thu ngân B",
                ReportParameterAccountant = "Kế toán C"
            };

            // =========================
            // 3. Template path
            // =========================
            string templatePath = Path.Combine(
                _env.WebRootPath,
                "templates",
                "Print",
                "ProductPrint.docx"
            );

            // =========================
            // 4. Load Word
            // =========================
            Document doc = new Document();
            doc.LoadFromFile(templatePath);

            // =========================
            // 5. Replace text
            // =========================
            ReplaceTemplate(doc, value);

            // =========================
            // 6. Fill table products
            // =========================
            FillProductTable(doc, products);

            // =========================
            // 7. Export PDF
            // =========================
            using var pdfStream = new MemoryStream();

            doc.SaveToStream(pdfStream, FileFormat.PDF);

            return pdfStream.ToArray();
        }


        // =======================================
        // Replace {{FieldName}}
        // =======================================
        private void ReplaceTemplate(Document doc, object data)
        {
            var properties = data.GetType().GetProperties();

            foreach (var prop in properties)
            {
                string placeholder = $"{{{{{prop.Name}}}}}";

                string value = prop.GetValue(data)?.ToString() ?? "";

                doc.Replace(
                    placeholder,
                    value,
                    true,
                    true
                );
            }
        }



        private void FillProductTable(Document doc, List<Product> products)
        {
            if (doc.Sections.Count == 0)
                throw new Exception("Word không có section");

            if (doc.Sections[0].Tables.Count < 2)
                throw new Exception("Không tìm thấy bảng sản phẩm");

            // Bảng thứ 2 mới là bảng sản phẩm
            Table table = doc.Sections[0].Tables[1] as Table;

            if (table.Rows.Count < 2)
                throw new Exception("Bảng sản phẩm phải có ít nhất 2 dòng");

            // Dòng header = row 0
            // Dòng template = row 1
            TableRow templateRow = table.Rows[1];

            foreach (var item in products.Select((p, index) => new { p, index }))
            {
                TableRow newRow = templateRow.Clone();

                newRow.Cells[0].Paragraphs[0].Text = (item.index + 1).ToString();
                newRow.Cells[1].Paragraphs[0].Text = item.p.Name ?? "";
                newRow.Cells[2].Paragraphs[0].Text = item.p.Price.ToString("N0");
                newRow.Cells[3].Paragraphs[0].Text = item.p.StockQuantity.ToString();
                newRow.Cells[4].Paragraphs[0].Text = item.p.MinimumAge + "+";
                newRow.Cells[5].Paragraphs[0].Text = item.p.Manufacturer ?? "";
                newRow.Cells[6].Paragraphs[0].Text =
                    item.p.Category?.CategoryName ?? "N/A";

                table.Rows.Add(newRow);
            }

            // Xóa dòng template
            table.Rows.Remove(templateRow);
        }



    }
}