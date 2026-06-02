using Microsoft.AspNetCore.Mvc;
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
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            return Ok(await _reviewService.GetReviewsByProductAsync(productId));
        }

        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRating(Guid productId)
        {
            return Ok(new { AverageRating = await _reviewService.GetAverageRatingAsync(productId) });
        }
    }
}
