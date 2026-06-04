using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using ToyStoreManagement.Application.Helpers;
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

        public ProductService(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.Category).OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Product>().GetQueryable()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<(bool Success, string Message, Product? Data)> CreateAsync(Product product, IFormFile? imageFile = null)
        {
            if (string.IsNullOrEmpty(product.Name)) return (false, "Name cannot be empty", null);
            if (product.Price < 0) return (false, "Price cannot be negative", null);

            product.ProductId = Guid.NewGuid();

            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                
                // Loại bỏ dấu tiếng Việt trong tên file
                string extension = Path.GetExtension(imageFile.FileName);
                string fileNameOnly = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string safeFileName = StringHelper.RemoveDiacritics(fileNameOnly) + extension;
                
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

                var categoryIdsInRequest = products.Select(p => p.CategoryId).Distinct().ToList();

                var existingCategories = await _unitOfWork.Repository<Category>().GetQueryable()
                    .Where(c => categoryIdsInRequest.Contains(c.CategoryId))
                    .ToDictionaryAsync<Category, Guid>(c => c.CategoryId);

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

            // Map manual update fields
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.MinimumAge = product.MinimumAge;
            existingProduct.Manufacturer = product.Manufacturer;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Badge = product.Badge;
            existingProduct.BadgeColor = product.BadgeColor;

            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                
                // Loại bỏ dấu tiếng Việt trong tên file
                string extension = Path.GetExtension(imageFile.FileName);
                string fileNameOnly = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string safeFileName = StringHelper.RemoveDiacritics(fileNameOnly) + extension;

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
    }
}
