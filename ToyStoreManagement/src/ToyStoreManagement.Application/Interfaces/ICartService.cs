using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs.Sales;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, AddToCartDto dto);
        Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(Guid cartItemId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}
