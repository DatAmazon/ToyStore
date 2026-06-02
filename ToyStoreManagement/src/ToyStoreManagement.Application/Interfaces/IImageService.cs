using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IImageService
    {
        Task<(bool Success, string Message)> UploadImageAsync(ProductImage image);
        Task<bool> SetMainImageAsync(Guid productId, Guid imageId);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<IEnumerable<ProductImage>> GetImagesByProductAsync(Guid productId);
    }
}
