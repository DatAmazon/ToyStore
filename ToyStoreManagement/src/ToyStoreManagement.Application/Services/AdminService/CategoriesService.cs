using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services.AdminService
{
    public class CategoriesService
    {
        private readonly IRepository<Category> _categoryRepo;

        public CategoriesService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepo.GetQueryable().ToListAsync();
        }

        public async Task<(bool Success, string Message, IEnumerable<Category>? Data)> CreateMultipleAsync(IEnumerable<Category> categories)
        {
            if (categories == null || !categories.Any())
                return (false, "Category list cannot be empty.", null);

            var categoryNames = categories.Select(c => c.CategoryName).ToList();
            var existingNames = await _categoryRepo.GetQueryable()
                .Where(c => categoryNames.Contains(c.CategoryName))
                .Select(c => c.CategoryName)
                .ToListAsync();

            if (existingNames.Any())
                return (false, $"The following categories already exist: {string.Join(", ", existingNames)}", null);

            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.CategoryName))
                    return (false, "One of the categories has an empty name.", null);

                category.CategoryId = Guid.NewGuid();
            }

            await _categoryRepo.AddRangeAsync(categories);
            await _categoryRepo.SaveChangesAsync();

            return (true, "Saved successfully!", categories);
        }
    }
}
