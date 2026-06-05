using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ProductImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload/{productId}")]
        public async Task<IActionResult> Upload(Guid productId, IFormFile file, [FromQuery] bool isMain = false)
        {
            if (file == null || file.Length == 0) 
                return BadRequest(ApiResponse<object>.FailureResponse("File is empty"));

            using var stream = file.OpenReadStream();
            var result = await _imageService.UploadProductImageAsync(
                productId, 
                stream, 
                file.FileName, 
                file.ContentType, 
                isMain);

            return result.Success 
                ? Ok(ApiResponse<object>.SuccessResponse(new { result.Url }, result.Message)) 
                : BadRequest(ApiResponse<object>.FailureResponse(result.Message));
        }

        [HttpPut("{productId}/main/{imageId}")]
        public async Task<IActionResult> SetMain(Guid productId, Guid imageId)
        {
            var success = await _imageService.SetMainImageAsync(productId, imageId);
            if (!success) 
                return NotFound(ApiResponse<object>.FailureResponse("Image or product not found"));
                
            return Ok(ApiResponse<object>.SuccessResponse(null, "Main image set successfully"));
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var success = await _imageService.DeleteImageAsync(imageId);
            if (!success) 
                return NotFound(ApiResponse<object>.FailureResponse("Image not found"));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Image deleted successfully"));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var data = await _imageService.GetImagesByProductAsync(productId);
            return Ok(ApiResponse<IEnumerable<ProductImage>>.SuccessResponse(data));
        }
    }
}
