using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var data = await _wishlistService.GetUserWishlistAsync(GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> Add(Guid productId)
        {
            await _wishlistService.AddToWishlistAsync(GetUserId(), productId);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã thêm vào danh sách yêu thích"));
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> Remove(Guid productId)
        {
            var result = await _wishlistService.RemoveFromWishlistAsync(GetUserId(), productId);
            return result 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Đã xóa khỏi danh sách yêu thích"))
                : BadRequest(ApiResponse<object>.FailureResponse("Sản phẩm không có trong danh sách yêu thích"));
        }

        [HttpGet("check/{productId}")]
        public async Task<IActionResult> Check(Guid productId)
        {
            var isIn = await _wishlistService.IsInWishlistAsync(GetUserId(), productId);
            return Ok(ApiResponse<object>.SuccessResponse(new { IsInWishlist = isIn }));
        }
    }
}
