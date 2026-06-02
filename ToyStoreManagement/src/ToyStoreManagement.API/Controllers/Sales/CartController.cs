using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetItems() => Ok(await _cartService.GetCartItemsAsync(GetUserId()));

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            await _cartService.RemoveFromCartAsync(cartItemId);
            return NoContent();
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal() => Ok(new { Total = await _cartService.GetCartTotalAsync(GetUserId()) });
    }
}
