using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services.AdminService
{
    public class ProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Category> _categoryRepo;

        public ProductService(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepo.GetQueryable()
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _productRepo.GetQueryable()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<(bool Success, string Message, Product? Data)> CreateAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) return (false, "Name cannot be empty", null);
            if (product.Price < 0) return (false, "Price cannot be negative", null);

            product.ProductId = Guid.NewGuid();
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return (true, "Success", product);
        }

        public async Task<(bool Success, string Message)> CreateWithAutoCategoryAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any()) return (false, "List is empty");

            // Note: Transaction management should ideally be moved to an IUnitOfWork.
            // For now, we are focusing on removing the AppDbContext dependency.
            try
            {
                var categoryIdsInRequest = products.Select(p => p.CategoryId).Distinct().ToList();

                var existingCategories = await _categoryRepo.GetQueryable()
                    .Where(c => categoryIdsInRequest.Contains(c.CategoryId))
                    .ToDictionaryAsync(c => c.CategoryId);

                foreach (var product in products)
                {
                    if (!existingCategories.ContainsKey(product.CategoryId))
                    {
                        var newCategory = new Category
                        {
                            CategoryId = product.CategoryId != Guid.Empty ? product.CategoryId : Guid.NewGuid(),
                            CategoryName = "New Auto-created Category"
                        };

                        await _categoryRepo.AddAsync(newCategory);
                        existingCategories.Add(newCategory.CategoryId, newCategory);
                    }
                    product.ProductId = Guid.NewGuid();
                    product.Category = null;
                }

                await _productRepo.AddRangeAsync(products);
                await _productRepo.SaveChangesAsync();

                return (true, "Processed successfully!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Bulk processing error: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var exists = await _productRepo.GetQueryable().AnyAsync(p => p.ProductId == product.ProductId);
            if (!exists) return false;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();
            return true;
        }
    }
}
