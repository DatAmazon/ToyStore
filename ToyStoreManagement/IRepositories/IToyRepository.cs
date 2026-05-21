using ProductStoreManagement.Entities;

namespace ToyStoreManagement.IRepositories
{
    public interface IToyRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetToysByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetLowStockToysAsync(int threshold);
    }
}
