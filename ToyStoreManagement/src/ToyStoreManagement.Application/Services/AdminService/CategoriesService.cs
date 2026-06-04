using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.Interfaces.IAdminService;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services.AdminService
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Category>().GetQueryable().ToListAsync();
        }

        public async Task<(bool Success, string Message, IEnumerable<Category>? Data)> CreateMultipleAsync(IEnumerable<Category> categories)
        {
            if (categories == null || !categories.Any())
                return (false, "Category list cannot be empty.", null);

            var categoryNames = categories.Select(c => c.CategoryName).ToList();
            var existingNames = await _unitOfWork.Repository<Category>().GetQueryable()
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

            await _unitOfWork.Repository<Category>().AddRangeAsync(categories);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Saved successfully!", categories);
        }
    }
}
