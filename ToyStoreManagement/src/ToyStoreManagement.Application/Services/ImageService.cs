using System;
using System.Collections.Generic;
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

        public ImageService(IRepository<ProductImage> imageRepo)
        {
            _imageRepo = imageRepo;
        }

        public async Task<(bool Success, string Message)> UploadImageAsync(ProductImage image)
        {
            image.Id = Guid.NewGuid();
            
            // Nếu đây là ảnh chính, đảm bảo các ảnh khác không phải ảnh chính
            if (image.IsMain)
            {
                await ResetMainImageAsync(image.ProductId);
            }

            await _imageRepo.AddAsync(image);
            await _imageRepo.SaveChangesAsync();
            return (true, "Image uploaded successfully.");
        }

        public async Task<bool> SetMainImageAsync(Guid productId, Guid imageId)
        {
            await ResetMainImageAsync(productId);

            var image = await _imageRepo.GetByIdAsync(imageId);
            if (image == null || image.ProductId != productId) return false;

            image.IsMain = true;
            await _imageRepo.SaveChangesAsync();
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
