using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces.IAdminService
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetAllAsync();
        Task<(bool Success, string Message)> CreateMultipleAsync(IEnumerable<CategoryDto> categoryDtos);
        Task<bool> UpdateAsync(CategoryDto categoryDto);
        Task<bool> DeleteAsync(Guid id);
        Task<Category?> GetByIdAsync(Guid id);
    }
}
