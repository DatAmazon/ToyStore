using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface IReviewService
    {
        Task<(bool Success, string Message)> AddReviewAsync(ProductReview review);
        Task<IEnumerable<ProductReview>> GetReviewsByProductAsync(Guid productId);
        Task<double> GetAverageRatingAsync(Guid productId);
    }
}
