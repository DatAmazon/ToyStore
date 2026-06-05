using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous_user";

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            await _cartService.AddToCartAsync(GetUserId(), dto);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Added to cart"));
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
            return Ok(ApiResponse<object>.SuccessResponse(null, "Removed from cart"));
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _cartService.GetCartTotalAsync(GetUserId());
            return Ok(ApiResponse<object>.SuccessResponse(new { Total = total }));
        }
    }
}
