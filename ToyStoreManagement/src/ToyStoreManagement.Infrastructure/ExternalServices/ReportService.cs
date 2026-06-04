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
                    Price = p.Price.ToString("N0") + " VNĐ",
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
    }
}

