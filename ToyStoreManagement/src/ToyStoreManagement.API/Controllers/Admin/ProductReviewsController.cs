using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ProductReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var reviews = await _reviewService.GetReviewsByProductAsync(productId);
            var average = await _reviewService.GetAverageRatingAsync(productId);
            return Ok(ApiResponse<object>.SuccessResponse(new { Reviews = reviews, AverageRating = average }));
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] ProductReview review)
        {
            // Tự động gán thông tin người dùng từ Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            review.UserId = userId;

            var result = await _reviewService.AddReviewAsync(review);
            return result.Success
                ? Ok(ApiResponse<object>.SuccessResponse(null, result.Message))
                : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
        }
    }
}
