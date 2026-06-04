using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IRepository<ProductImage> _imageRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IStorageService _storageService;

        public ImageService(IRepository<ProductImage> imageRepo, IRepository<Product> productRepo, IStorageService storageService)
        {
            _imageRepo = imageRepo;
            _productRepo = productRepo;
            _storageService = storageService;
        }

        public async Task<(bool Success, string Message, string? Url)> UploadProductImageAsync(Guid productId, Stream fileStream, string fileName, string contentType, bool isMain = false)
        {
            try
            {
                // 1. Tải ảnh lên MinIO
                string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                string fileKey = await _storageService.UploadFileAsync(fileStream, uniqueFileName, contentType);
                string imageUrl = _storageService.GetFileUrl(fileKey);

                // 2. Lưu thông tin vào Database
                var productImage = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    IsMain = isMain
                };

                if (isMain)
                {
                    await ResetMainImageAsync(productId);
                    
                    // Cập nhật ảnh chính cho Product entity
                    var product = await _productRepo.GetByIdAsync(productId);
                    if (product != null)
                    {
                        product.ImageUrl = imageUrl;
                    }
                }

                await _imageRepo.AddAsync(productImage);
                await _imageRepo.SaveChangesAsync();
                await _productRepo.SaveChangesAsync();

                return (true, "Image uploaded successfully.", imageUrl);
            }
            catch (Exception ex)
            {
                return (false, $"Error uploading image: {ex.Message}", null);
            }
        }

        public async Task<bool> SetMainImageAsync(Guid productId, Guid imageId)
        {
            await ResetMainImageAsync(productId);

            var image = await _imageRepo.GetByIdAsync(imageId);
            if (image == null || image.ProductId != productId) return false;

            image.IsMain = true;

            // Cập nhật ảnh chính cho Product entity
            var product = await _productRepo.GetByIdAsync(productId);
            if (product != null)
            {
                product.ImageUrl = image.ImageUrl;
            }

            await _imageRepo.SaveChangesAsync();
            await _productRepo.SaveChangesAsync();
            return true;
        }

        private async Task ResetMainImageAsync(Guid productId)
        {
            var images = await _imageRepo.GetQueryable()
                .Where(i => i.ProductId == productId && i.IsMain)
                .ToListAsync();

            foreach (var img in images)
            {
                img.IsMain = false;
            }
            await _imageRepo.SaveChangesAsync();
        }

        public async Task<bool> DeleteImageAsync(Guid imageId)
        {
            var image = await _imageRepo.GetByIdAsync(imageId);
            if (image == null) return false;

            // Xóa file trên MinIO (giả định ImageUrl chứa key hoặc có thể trích xuất key)
            // Trong thực tế, bạn nên lưu FileKey riêng biệt.
            // Ở đây tôi sẽ cố gắng trích xuất tên file từ URL nếu có thể, hoặc bỏ qua nếu phức tạp.
            // Giả sử URL có dạng: http://.../bucket/folder/filename
            string fileKey = image.ImageUrl.Split('/').Last();
            await _storageService.DeleteFileAsync($"toystore/{fileKey}");

            _imageRepo.Delete(image);
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductImage>> GetImagesByProductAsync(Guid productId)
        {
            return await _imageRepo.GetQueryable()
                .Where(i => i.ProductId == productId)
                .OrderByDescending(i => i.IsMain)
                .ToListAsync();
        }
    }
}
