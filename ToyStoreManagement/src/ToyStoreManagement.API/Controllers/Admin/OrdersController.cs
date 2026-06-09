using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Constants;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleService _saleService;

        public OrdersController(IUnitOfWork unitOfWork, ISaleService saleService)
        {
            _unitOfWork = unitOfWork;
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _unitOfWork.Repository<Order>().GetQueryable()
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    TotalAmount = o.TotalAmount,
                    Discount = o.Discount,
                    FinalAmount = o.FinalAmount,
                    OrderDate = o.OrderDate,
                    Status = o.Status
                })
                .ToListAsync();

            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var order = await _saleService.GetOrderDetailsAsync(id);
                return Ok(ApiResponse<OrderResponseDto>.SuccessResponse(order));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string newStatus)
        {
            var validStatuses = new[] 
            { 
                OrderStatuses.Pending, 
                OrderStatuses.Confirmed, 
                OrderStatuses.Processing, 
                OrderStatuses.Shipping, 
                OrderStatuses.Delivered, 
                OrderStatuses.Cancelled 
            };
            
            if (!validStatuses.Contains(newStatus))
                return BadRequest(ApiResponse<object>.FailureResponse("Trạng thái không hợp lệ."));

            var result = await _saleService.UpdateOrderStatusAsync(id, newStatus);
            
            return result 
                ? Ok(ApiResponse<object>.SuccessResponse(null, $"Cập nhật trạng thái đơn hàng thành: {newStatus}"))
                : NotFound(ApiResponse<object>.FailureResponse("Không tìm thấy đơn hàng."));
        }
    }
}
