using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId() 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
            return userId;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            await _cartService.AddToCartAsync(GetUserId(), dto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã thêm vào giỏ hàng"));
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            await _cartService.UpdateQuantityAsync(GetUserId(), dto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã cập nhật số lượng"));
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var data = await _cartService.GetCartItemsAsync(GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(data));
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            await _cartService.RemoveFromCartAsync(cartItemId);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã xóa khỏi giỏ hàng"));
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _cartService.GetCartTotalAsync(GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(new { Total = total }));
        }

        [HttpDelete("clear-cart")]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearCartAsync(GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(null, "Đã làm trống giỏ hàng"));
        }
    }
}
