using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Application.Interfaces.IAdminService;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services.AdminService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IMemoryCache _cache;

        public ProductService(IUnitOfWork unitOfWork, IStorageService storageService, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _cache = cache;
        }

        public async Task<List<Product>> SearchAndFilterAsync(string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice, string? sortOrder, int pageNumber = 1, int pageSize = 24)
        {
            string cacheKey = $"Search_K:{keyword}_C:{categoryId}_Min:{minPrice}_Max:{maxPrice}_Sort:{sortOrder}_P:{pageNumber}_S:{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out List<Product>? cachedProducts))
            {
                // Ép kiểu tường minh ngay từ đầu để tránh lỗi TEntity trong IDE
                IQueryable<Product> baseQuery = _unitOfWork.Repository<Product>().GetQueryable();
                
                var query = baseQuery
                    .Where((Product p) => !p.IsDeleted)
                    .Include((Product p) => p.Category)
                    .AsNoTracking();

                if (!string.IsNullOrEmpty(keyword))
                {
                    string unaccentedKeyword = ToyStoreManagement.Application.Helpers.StringHelper.Unaccent(keyword);
                    query = query.Where((Product p) => (p.SearchName != null && EF.Functions.Like(p.SearchName, $"%{unaccentedKeyword}%")) || 
                                             EF.Functions.Like(p.Name.ToLower(), $"%{keyword.ToLower()}%"));
                }

                if (categoryId.HasValue && categoryId != Guid.Empty)
                {
                    query = query.Where((Product p) => p.CategoryId == categoryId.Value);
                }

                if (minPrice.HasValue)
                    query = query.Where((Product p) => (p.DiscountPrice ?? p.Price) >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where((Product p) => (p.DiscountPrice ?? p.Price) <= maxPrice.Value);

                // Sorting
                query = sortOrder?.ToLower() switch
                {
                    "price_asc" => query.OrderBy((Product p) => p.DiscountPrice ?? p.Price),
                    "price_desc" => query.OrderByDescending((Product p) => p.DiscountPrice ?? p.Price),
                    "newest" => query.OrderByDescending((Product p) => p.ProductId),
                    _ => query.OrderBy((Product p) => p.Name),
                };

                cachedProducts = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, cachedProducts, cacheEntryOptions);
            }

            return cachedProducts ?? new List<Product>();
        }

        public async Task<List<Product>> GetAllToysAsync()
        {
            return await _unitOfWork.Repository<Product>().GetQueryable()
                .Include((Product p) => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> GetLowStockToysAsync(int threshold)
        {
            return await _unitOfWork.Repository<Product>().GetQueryable()
                .Where((Product p) => p.StockQuantity < threshold)
                .Include((Product p) => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Product>().GetQueryable()
                .Include((Product p) => p.Category)
                .FirstOrDefaultAsync((Product p) => p.ProductId == id);
        }

        public async Task<(bool Success, string Message, Product? Data)> CreateAsync(Product product, IFormFile? imageFile = null)
        {
            if (string.IsNullOrEmpty(product.Name)) return (false, "Name cannot be empty", null);
            if (product.Price < 0) return (false, "Price cannot be negative", null);

            // TỰ ĐỘNG TÍNH TOÁN GIẢM GIÁ
            if (product.DiscountPercentage.HasValue && product.DiscountPercentage > 0)
            {
                // Ưu tiên tính theo % nếu có
                product.DiscountPrice = product.Price * (1 - (decimal)product.DiscountPercentage.Value / 100);
            }
            else if (product.DiscountPrice.HasValue && product.DiscountPrice > 0)
            {
                // Nếu chỉ có giá tiền, tính ngược lại % để hiển thị nhãn
                product.DiscountPercentage = (int)((product.Price - product.DiscountPrice.Value) / product.Price * 100);
            }

            product.ProductId = Guid.NewGuid();
            product.SearchName = ToyStoreManagement.Application.Helpers.StringHelper.Unaccent(product.Name);

            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                
                string extension = Path.GetExtension(imageFile.FileName);
                string fileNameOnly = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string safeFileName = ToyStoreManagement.Application.Helpers.StringHelper.RemoveDiacritics(fileNameOnly) + extension;
                
                string fileName = $"{Guid.NewGuid()}_{safeFileName}";
                string fileKey = await _storageService.UploadFileAsync(stream, fileName, imageFile.ContentType);
                product.ImageUrl = _storageService.GetFileUrl(fileKey);
            }

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Success", product);
        }

        public async Task<(bool Success, string Message)> CreateWithAutoCategoryAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any()) return (false, "List is empty");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var categoryIdsInRequest = products.Select((Product p) => p.CategoryId).Distinct().ToList();

                var existingCategories = await _unitOfWork.Repository<Category>().GetQueryable()
                    .Where((Category c) => categoryIdsInRequest.Contains(c.CategoryId))
                    .ToDictionaryAsync<Category, Guid>((Category c) => c.CategoryId);

                foreach (var product in products)
                {
                    if (!existingCategories.ContainsKey(product.CategoryId))
                    {
                        var newCategory = new Category
                        {
                            CategoryId = product.CategoryId != Guid.Empty ? product.CategoryId : Guid.NewGuid(),
                            CategoryName = "New Auto-created Category"
                        };

                        await _unitOfWork.Repository<Category>().AddAsync(newCategory);
                        existingCategories.Add(newCategory.CategoryId, newCategory);
                    }
                    product.ProductId = Guid.NewGuid();
                    product.SearchName = ToyStoreManagement.Application.Helpers.StringHelper.Unaccent(product.Name);
                    product.Category = null;
                }

                await _unitOfWork.Repository<Product>().AddRangeAsync(products);
                await _unitOfWork.CommitTransactionAsync();

                return (true, "Processed successfully!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Bulk processing error: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(Product product, IFormFile? imageFile = null)
        {
            var existingProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(product.ProductId);
            if (existingProduct == null) return false;

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;

            // TỰ ĐỘNG TÍNH TOÁN GIẢM GIÁ
            if (product.DiscountPercentage.HasValue && product.DiscountPercentage > 0)
            {
                existingProduct.DiscountPercentage = product.DiscountPercentage;
                existingProduct.DiscountPrice = product.Price * (1 - (decimal)product.DiscountPercentage.Value / 100);
            }
            else if (product.DiscountPrice.HasValue && product.DiscountPrice > 0)
            {
                existingProduct.DiscountPrice = product.DiscountPrice;
                existingProduct.DiscountPercentage = (int)((product.Price - product.DiscountPrice.Value) / product.Price * 100);
            }
            else
            {
                existingProduct.DiscountPrice = null;
                existingProduct.DiscountPercentage = null;
            }

            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.MinimumAge = product.MinimumAge;
            existingProduct.Manufacturer = product.Manufacturer;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Badge = product.Badge;
            existingProduct.BadgeColor = product.BadgeColor;
            existingProduct.SearchName = ToyStoreManagement.Application.Helpers.StringHelper.Unaccent(product.Name);

            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                
                string extension = Path.GetExtension(imageFile.FileName);
                string fileNameOnly = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string safeFileName = ToyStoreManagement.Application.Helpers.StringHelper.RemoveDiacritics(fileNameOnly) + extension;

                string fileName = $"{Guid.NewGuid()}_{safeFileName}";
                string fileKey = await _storageService.UploadFileAsync(stream, fileName, imageFile.ContentType);
                existingProduct.ImageUrl = _storageService.GetFileUrl(fileKey);
            }

            _unitOfWork.Repository<Product>().Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return false;

            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 30)
        {
            throw new NotImplementedException();
        }
    }
}
