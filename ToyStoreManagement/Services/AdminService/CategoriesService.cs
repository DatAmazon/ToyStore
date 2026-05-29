using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Data;
using ToyStoreManagement.Entities;

namespace ToyStoreManagement.Services.AdminService
{
    public class CategoriesService
    {
        private readonly AppDbContext _context;

        public CategoriesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<(bool Success, string Message, IEnumerable<Category> Data)> CreateMultipleAsync(IEnumerable<Category> categories)
        {
            if (categories == null || !categories.Any())
                return (false, "Danh sách danh mục không được để trống.", null);

            // 1. Kiểm tra trùng lặp tên
            var categoryNames = categories.Select(c => c.CategoryName).ToList();
            var existingNames = await _context.Categories
                .Where(c => categoryNames.Contains(c.CategoryName))
                .Select(c => c.CategoryName)
                .ToListAsync();

            if (existingNames.Any())
                return (false, $"Các danh mục sau đã tồn tại: {string.Join(", ", existingNames)}", null);

            // 2. Xử lý logic và gán Guid
            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.CategoryName))
                    return (false, "Có danh mục trong danh sách bị trống tên.", null);

                category.CategoryId = Guid.NewGuid();
            }

            // 3. Thực thi lưu
            await _context.Categories.AddRangeAsync(categories);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
                return (true, $"Lưu thành công {result} danh mục!", categories);

            return (false, "Lệnh chạy nhưng không có dòng nào được lưu.", null);
        }
    }
}
