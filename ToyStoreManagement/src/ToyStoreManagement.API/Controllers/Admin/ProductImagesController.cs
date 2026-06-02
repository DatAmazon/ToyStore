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

        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] ProductImage image)
        {
            var result = await _imageService.UploadImageAsync(image);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
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
