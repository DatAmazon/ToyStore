using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderRequestDto request)
        {
            if (request == null || !request.Items.Any())
                return BadRequest("Đơn hàng không có sản phẩm nào.");

            var result = await _saleService.CheckoutAsync(request);
            return Ok(new
            {
                Success = true,
                Message = "Đặt hàng thành công!",
                OrderId = result
            });
        }

    }
}
