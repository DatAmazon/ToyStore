using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ProductReview review)
        {
            var result = await _reviewService.AddReviewAsync(review);
            return result.Success 
                ? Ok(ApiResponse<object>.SuccessResponse(null, result.Message)) 
                : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var data = await _reviewService.GetReviewsByProductAsync(productId);
            return Ok(ApiResponse<IEnumerable<ProductReview>>.SuccessResponse(data));
        }

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRating(Guid productId)
        {
            var average = await _reviewService.GetAverageRatingAsync(productId);
            return Ok(ApiResponse<object>.SuccessResponse(new { AverageRating = average }));
        }
    }
}
