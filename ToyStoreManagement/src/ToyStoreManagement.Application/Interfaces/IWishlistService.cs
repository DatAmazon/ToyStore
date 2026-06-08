using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(string userId, Guid productId);
        Task<bool> RemoveFromWishlistAsync(string userId, Guid productId);
        Task<IEnumerable<ProductDto>> GetUserWishlistAsync(string userId);
        Task<bool> IsInWishlistAsync(string userId, Guid productId);
    }
}
