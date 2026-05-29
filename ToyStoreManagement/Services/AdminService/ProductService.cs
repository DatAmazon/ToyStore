using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
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

            string templatePath = Path.Combine(_env.WebRootPath, "templates", "ProductTemplate.xlsx");

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
    }
}
