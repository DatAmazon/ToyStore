using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Domain.Entities;

using System.IO;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IImageService
    {
        Task<(bool Success, string Message, string? Url)> UploadProductImageAsync(Guid productId, Stream fileStream, string fileName, string contentType, bool isMain = false);
        Task<bool> SetMainImageAsync(Guid productId, Guid imageId);
        Task<bool> DeleteImageAsync(Guid imageId);
        Task<IEnumerable<ProductImage>> GetImagesByProductAsync(Guid productId);
    }
}
