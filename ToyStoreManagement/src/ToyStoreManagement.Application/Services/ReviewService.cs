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
    public class ReviewService : IReviewService
    {
        private readonly IRepository<ProductReview> _reviewRepo;

        public ReviewService(IRepository<ProductReview> reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<(bool Success, string Message)> AddReviewAsync(ProductReview review)
        {
            if (review.Rating < 1 || review.Rating > 5)
                return (false, "Rating must be between 1 and 5.");

            review.Id = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
            return (true, "Review added successfully.");
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductAsync(Guid productId)
        {
            return await _reviewRepo.GetQueryable()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid productId)
        {
            var reviews = await _reviewRepo.GetQueryable()
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }
    }
}
