using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Entities;

namespace ToyStoreManagement.IRepositories
{
    public interface IToyRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetToysByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetLowStockToysAsync(int threshold);
    }
}
