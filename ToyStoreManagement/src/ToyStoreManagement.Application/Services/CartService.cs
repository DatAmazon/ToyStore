using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

using ToyStoreManagement.Domain.Constants;

namespace ToyStoreManagement.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _cartRepo;
        private readonly IRepository<Product> _productRepo;

        public CartService(IRepository<CartItem> cartRepo, IRepository<Product> productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task AddToCartAsync(string userId, AddToCartDto dto)
        {
            var cartItem = await _cartRepo.GetQueryable()
                .FirstOrDefaultAsync(c => c.CustomerId == userId && c.ProductId == dto.ProductId && c.Status == dto.Status);

            if (cartItem != null)
            {
                cartItem.Quantity += dto.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CustomerId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Status = CartItemStatuses.Active
                };
                await _cartRepo.AddAsync(cartItem);
            }
            await _cartRepo.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            var cartItem = await _cartRepo.GetQueryable()
                .FirstOrDefaultAsync(c => c.CartItemId == dto.CartItemId && c.CustomerId == userId);

            if (cartItem == null) throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ hàng.");

            if (dto.Quantity <= 0)
            {
                _cartRepo.Delete(cartItem);
            }
            else
            {
                cartItem.Quantity = dto.Quantity;
            }
            await _cartRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsAsync(string userId)
        {
            return await _cartRepo.GetQueryable()
                .Where(c => c.CustomerId == userId)
                .Include(c => c.Product)
                .Select(c => new CartItemDto
                {
                    Id = c.CartItemId,
                    ProductId = c.ProductId,
                    ProductName = c.Product != null ? c.Product.Name : "N/A",
                    Price = c.Product != null ? c.Product.Price : 0,
                    Quantity = c.Quantity,
                    Status = c.Status
                })
                .ToListAsync();
        }

        public async Task RemoveFromCartAsync(Guid cartItemId)
        {
            var item = await _cartRepo.GetQueryable().FirstOrDefaultAsync(c => c.CartItemId == cartItemId);
            if (item != null)
            {
                _cartRepo.Delete(item);
                await _cartRepo.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var items = await _cartRepo.GetQueryable().Where(c => c.CustomerId == userId).ToListAsync();
            foreach (var item in items) _cartRepo.Delete(item);
            await _cartRepo.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var items = await _cartRepo.GetQueryable()
                .Where(c => c.CustomerId == userId && c.Status == CartItemStatuses.Active)
                .Include(c => c.Product)
                .ToListAsync();

            return items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity);
        }
    }
}
