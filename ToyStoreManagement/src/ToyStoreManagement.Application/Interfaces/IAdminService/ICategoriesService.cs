using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces.IAdminService
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetAllAsync();
        Task<(bool Success, string Message, IEnumerable<Category>? Data)> CreateMultipleAsync(IEnumerable<Category> categories);
    }
}
