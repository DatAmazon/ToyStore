using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs;
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

        public async Task<(bool Success, string Message)> CreateMultipleAsync(IEnumerable<Category> categories)
        {
            if (categories == null || !categories.Any())
                return (false, "Category list cannot be empty.");

            var categoryNames = categories.Select(c => c.CategoryName).ToList();
            var existingNames = await _unitOfWork.Repository<Category>().GetQueryable()
                .Where(c => categoryNames.Contains(c.CategoryName))
                .Select(c => c.CategoryName)
                .ToListAsync();

            if (existingNames.Any())
                return (false, $"The following categories already exist: {string.Join(", ", existingNames)}");

            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.CategoryName))
                    return (false, "One of the categories has an empty name.");

                category.CategoryId = Guid.NewGuid();
            }

            await _unitOfWork.Repository<Category>().AddRangeAsync(categories);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Saved successfully!");
        }

        public async Task<bool> UpdateAsync(CategoryDto categoryDto)
        {
            var existing = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryDto.Id);
            if (existing == null) 
                throw new KeyNotFoundException($"Không tìm thấy danh mục với mã ID: {categoryDto.Id}");

            existing.CategoryName = categoryDto.CategoryName;

            _unitOfWork.Repository<Category>().Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        }
    }
}
