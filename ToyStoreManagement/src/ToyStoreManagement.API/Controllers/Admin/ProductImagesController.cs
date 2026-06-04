using Microsoft.AspNetCore.Mvc;
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
            if (file == null || file.Length == 0) return BadRequest("File is empty");

            using var stream = file.OpenReadStream();
            var result = await _imageService.UploadProductImageAsync(
                productId, 
                stream, 
                file.FileName, 
                file.ContentType, 
                isMain);

            return result.Success 
                ? Ok(new { result.Message, result.Url }) 
                : BadRequest(result.Message);
        }

        [HttpPut("{productId}/main/{imageId}")]
        public async Task<IActionResult> SetMain(Guid productId, Guid imageId)
        {
            var success = await _imageService.SetMainImageAsync(productId, imageId);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(Guid imageId)
        {
            var success = await _imageService.DeleteImageAsync(imageId);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            return Ok(await _imageService.GetImagesByProductAsync(productId));
        }
    }
}
