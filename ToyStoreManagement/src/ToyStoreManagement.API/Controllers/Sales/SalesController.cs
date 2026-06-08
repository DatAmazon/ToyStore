using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        public SalesController(ISaleService saleService) => _saleService = saleService;

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto searchDto)
        {
            var products = await _saleService.SearchProductsAsync(searchDto);
            return Ok(ApiResponse<object>.SuccessResponse(products));
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderRequestDto request)
        {
            if (request == null || !request.Items.Any())
                return BadRequest(ApiResponse<object>.FailureResponse("Đơn hàng không có sản phẩm nào."));

            // Nếu người dùng đã đăng nhập, tự động gán CustomerId từ Token
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid customerGuid))
            {
                request.CustomerId = customerGuid;
            }

            var result = await _saleService.CheckoutAsync(request);
            return Ok(ApiResponse<object>.SuccessResponse(new { OrderId = result }, "Đặt hàng thành công!"));
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var orders = await _saleService.GetCustomerOrdersAsync(userId);
            return Ok(ApiResponse<object>.SuccessResponse(orders));
        }

        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            var order = await _saleService.GetOrderDetailsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(order));
        }

        [HttpPost("cancel/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await _saleService.CancelOrderAsync(id);
            return result 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Đã hủy đơn hàng thành công."))
                : BadRequest(ApiResponse<object>.FailureResponse("Không thể hủy đơn hàng này."));
        }

        [HttpPost("payment/{id}")]
        public async Task<IActionResult> ProcessPayment(Guid id)
        {
            try
            {
                var result = await _saleService.ProcessPaymentAsync(id);
                return result
                    ? Ok(ApiResponse<object>.SuccessResponse(null, "Thanh toán thành công và đơn hàng đã được xác nhận."))
                    : BadRequest(ApiResponse<object>.FailureResponse("Thanh toán thất bại."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
        }
    }
}
