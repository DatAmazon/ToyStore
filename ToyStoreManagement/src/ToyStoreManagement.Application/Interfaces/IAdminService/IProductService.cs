using Microsoft.AspNetCore.Http;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces.IAdminService
{
    public interface IProductService
    {
        Task<List<Product>> GetAllToysAsync();
        Task<List<Product>> GetLowStockToysAsync(int threshold);
        Task<List<Product>> SearchAndFilterAsync(string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? sortOrder, int pageNumber = 1, int pageSize = 24);
        Task<Product?> GetByIdAsync(Guid id);
        Task<(bool Success, string Message, Product? Data)> CreateAsync(Product product, IFormFile? imageFile = null);
        Task<(bool Success, string Message)> CreateWithAutoCategoryAsync(IEnumerable<Product> products);
        Task<bool> UpdateAsync(Product product, IFormFile? imageFile = null);
        Task<bool> DeleteAsync(Guid id);
    }
}
