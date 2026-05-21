using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Data;
using ToyStoreManagement.Entities;
using ToyStoreManagement.IRepositories;

namespace ToyStoreManagement.Repositories
{
    public class ToyRepository : Repository<Toy>, IToyRepository
    {
        public ToyRepository(AppDbContext context) : base(context)
        {
        }

        // Lấy danh sách đồ chơi theo khoảng giá
        public async Task<IEnumerable<Toy>> GetToysByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Toys
                .Where(t => t.Price >= minPrice && t.Price <= maxPrice)
                .OrderBy(t => t.Price)
                .ToListAsync();
        }

        // Lấy danh sách đồ chơi có số lượng tồn kho thấp hơn ngưỡng cho trước
        public async Task<IEnumerable<Toy>> GetLowStockToysAsync(int threshold)
        {
            return await _context.Toys
                .Where(t => t.StockQuantity < threshold)
                .OrderBy(t => t.StockQuantity)
                .ToListAsync();
        }
    }
}
