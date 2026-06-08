using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<Wishlist> _wishlistRepo;
        private readonly IMapper _mapper;

        public WishlistService(IRepository<Wishlist> wishlistRepo, IMapper mapper)
        {
            _wishlistRepo = wishlistRepo;
            _mapper = mapper;
        }

        public async Task<bool> AddToWishlistAsync(string userId, Guid productId)
        {
            var existing = await _wishlistRepo.GetQueryable()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing != null) return true;

            var wish = new Wishlist
            {
                WishlistId = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                AddedDate = DateTime.UtcNow
            };

            await _wishlistRepo.AddAsync(wish);
            await _wishlistRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromWishlistAsync(string userId, Guid productId)
        {
            var existing = await _wishlistRepo.GetQueryable()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing == null) return false;

            _wishlistRepo.Delete(existing);
            await _wishlistRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetUserWishlistAsync(string userId)
        {
            var wishlist = await _wishlistRepo.GetQueryable()
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Select(w => w.Product)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(wishlist);
        }

        public async Task<bool> IsInWishlistAsync(string userId, Guid productId)
        {
            return await _wishlistRepo.GetQueryable()
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}
