using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ToyStoreManagement.Application.Helpers;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Infrastructure.ExternalServices
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _rootPath;

        public ReportService(IUnitOfWork unitOfWork, string rootPath)
        {
            _unitOfWork = unitOfWork;
            _rootPath = rootPath;
            // Cấu hình License cho QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GetProductExcelReportAsync()
        {
            string templatePath = Path.Combine(_rootPath, "templates", "Report", "ProductReport.xlsx");
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Template Excel không tồn tại.", templatePath);

            var products = await _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.Category)
                .AsNoTracking()
                .Select(p => new
                {
                    Name = p.Name,
                    OriginalPrice = p.Price.ToString("N0") + " VNĐ",
                    DiscountPrice = p.DiscountPrice.HasValue ? p.DiscountPrice.Value.ToString("N0") + " VNĐ" : "N/A",
                    DiscountPercentage = p.DiscountPercentage.HasValue ? p.DiscountPercentage.Value + "%" : "0%",
                    StockQuantity = p.StockQuantity,
                    MinimumAge = p.MinimumAge,
                    Manufacturer = p.Manufacturer,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "N/A"
                })
                .ToListAsync();

            var templateData = new { items = products };

            using var memoryStream = new MemoryStream();
            await memoryStream.SaveAsByTemplateAsync(templatePath, templateData);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GetProductPdfReportAsync(string title, string creatorName)
        {
            string templatePath = Path.Combine(_rootPath, "templates", "Print", "ProductPrint.docx");
            // Note: QuestPDF doesn't necessarily use docx as template, but the original code checked for it.
            // In the original code, it seems it was just checking if the file exists before generating PDF manually.

            var products = await _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            decimal totalValue = products.Sum(p => p.Price * p.StockQuantity);
            string totalValueInWords = CurrencyHelper.ToVietnameseWords(totalValue);

            var document = Document.Create(container =>
            {
                // ... (QuestPDF logic - same as before)
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(title).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).Italic();
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("TOY STORE MANAGEMENT").FontSize(12).SemiBold();
                            col.Item().Text("Hệ thống quản lý cửa hàng đồ chơi chuyên nghiệp").FontSize(8);
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("STT");
                                header.Cell().Element(CellStyle).Text("Tên sản phẩm");
                                header.Cell().Element(CellStyle).Text("Đơn giá");
                                header.Cell().Element(CellStyle).Text("Tồn kho");
                                header.Cell().Element(CellStyle).Text("Độ tuổi");
                                header.Cell().Element(CellStyle).Text("Nhà SX");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold())
                                                    .PaddingVertical(5)
                                                    .BorderBottom(1)
                                                    .BorderColor(Colors.Black);
                                }
                            });

                            // Table Rows
                            for (int i = 0; i < products.Count; i++)
                            {
                                var p = products[i];
                                table.Cell().Element(RowStyle).Text((i + 1).ToString());
                                table.Cell().Element(RowStyle).Text(p.Name ?? "");
                                table.Cell().Element(RowStyle).AlignRight().Text(p.Price.ToString("N0"));
                                table.Cell().Element(RowStyle).AlignCenter().Text(p.StockQuantity.ToString());
                                table.Cell().Element(RowStyle).AlignCenter().Text(p.MinimumAge + "+");
                                table.Cell().Element(RowStyle).Text(p.Manufacturer ?? "");

                                static IContainer RowStyle(IContainer container)
                                {
                                    return container.BorderBottom(1)
                                                    .BorderColor(Colors.Grey.Lighten2)
                                                    .PaddingVertical(5);
                                }
                            }
                        });

                        col.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(t =>
                                {
                                    t.Span("Tổng giá trị tồn kho: ").SemiBold();
                                    t.Span(totalValue.ToString("N0") + " VNĐ").FontColor(Colors.Red.Medium).SemiBold();
                                });
                                c.Item().Text(t =>
                                {
                                    t.Span("Bằng chữ: ").SemiBold().Italic();
                                    t.Span(totalValueInWords).Italic();
                                });
                            });
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GetOrderExcelReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.Repository<Order>().GetQueryable().AsNoTracking();

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    Mã_Đơn_Hàng = o.OrderId,
                    Ngày_Đặt = o.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                    Khách_Hàng = o.CustomerName,
                    SĐT = o.CustomerPhone,
                    Địa_Chỉ = o.ShippingAddress,
                    Tổng_Tiền = o.TotalAmount,
                    Giảm_Giá = o.Discount,
                    Thanh_Toán = o.FinalAmount,
                    Trạng_Thái = o.Status
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            memoryStream.SaveAs(orders);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GetInvoicePdfAsync(Guid orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetQueryable()
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("HÓA ĐƠN BÁN HÀNG").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Mã đơn: {order.OrderId}").FontSize(9);
                            col.Item().Text($"Ngày: {order.OrderDate:dd/MM/yyyy HH:mm}").FontSize(9);
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("TOY STORE MANAGEMENT").FontSize(14).SemiBold();
                            col.Item().Text("123 Phố Đồ Chơi, Hà Nội").FontSize(9);
                            col.Item().Text("Hotline: 1900 1234").FontSize(9);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Customer Info
                        col.Item().Border(1).Padding(10).Column(c =>
                        {
                            c.Item().Text("THÔNG TIN KHÁCH HÀNG").SemiBold();
                            c.Item().Text($"Họ tên: {order.CustomerName}");
                            c.Item().Text($"SĐT: {order.CustomerPhone}");
                            c.Item().Text($"Địa chỉ: {order.ShippingAddress}");
                        });

                        col.Item().PaddingVertical(10);

                        // Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("STT");
                                header.Cell().Element(CellStyle).Text("Sản phẩm");
                                header.Cell().Element(CellStyle).AlignRight().Text("Đơn giá");
                                header.Cell().Element(CellStyle).AlignCenter().Text("SL");
                                header.Cell().Element(CellStyle).AlignRight().Text("Thành tiền");

                                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1);
                            });

                            int i = 1;
                            foreach (var item in order.Details)
                            {
                                table.Cell().Element(RowStyle).Text(i++.ToString());
                                table.Cell().Element(RowStyle).Text(item.Product?.Name ?? "Sản phẩm không xác định");
                                table.Cell().Element(RowStyle).AlignRight().Text(item.Price.ToString("N0"));
                                table.Cell().Element(RowStyle).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Element(RowStyle).AlignRight().Text((item.Price * item.Quantity).ToString("N0"));

                                static IContainer RowStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        });

                        // Totals
                        col.Item().AlignRight().PaddingTop(10).Column(c =>
                        {
                            c.Item().Text($"Tổng tiền hàng: {order.TotalAmount:N0} VNĐ");
                            c.Item().Text($"Giảm giá: {order.Discount:N0} VNĐ");
                            c.Item().Text(t =>
                            {
                                t.Span("TỔNG THANH TOÁN: ").SemiBold();
                                t.Span($"{order.FinalAmount:N0} VNĐ").SemiBold().FontSize(14).FontColor(Colors.Red.Medium);
                            });
                            c.Item().PaddingTop(5).Text($"Bằng chữ: {CurrencyHelper.ToVietnameseWords(order.FinalAmount)}").Italic();
                        });
                    });

                    page.Footer().AlignCenter().Column(c =>
                    {
                        c.Item().Text("Cảm ơn Quý khách đã mua sắm tại Toy Store!").Italic();
                        c.Item().Text(x =>
                        {
                            x.Span("Trang ");
                            x.CurrentPageNumber();
                        });
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GetSalesReportExcelAsync(DateTime fromDate, DateTime toDate)
        {
            var salesData = await _unitOfWork.Repository<Order>().GetQueryable()
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate && o.Status != "Cancelled")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Ngày = g.Key.ToString("dd/MM/yyyy"),
                    Số_Đơn_Hàng = g.Count(),
                    Tổng_Doanh_Thu = g.Sum(o => o.FinalAmount)
                })
                .OrderBy(x => x.Ngày)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            memoryStream.SaveAs(salesData);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GetCustomerReportExcelAsync()
        {
            var customers = await _unitOfWork.Repository<Customer>().GetQueryable()
                .AsNoTracking()
                .Select(c => new
                {
                    Tên_Khách_Hàng = c.FullName,
                    Số_Điện_Thoại = c.Phone,
                    Email = c.Email,
                    Ngày_Đăng_Ký = c.CreatedAt.ToString("dd/MM/yyyy")
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            memoryStream.SaveAs(customers);
            return memoryStream.ToArray();
        }
    }
}

