using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Domain.Constants;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _unitOfWork.Repository<Order>().GetQueryable()
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.FinalAmount,
                    o.OrderDate,
                    o.Status
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(orders));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string newStatus)
        {
            var validStatuses = new[] { OrderStatuses.Pending, OrderStatuses.Confirmed, "Shipping", "Delivered", OrderStatuses.Cancelled, "Completed" };
            
            if (!validStatuses.Contains(newStatus))
                return BadRequest(ApiResponse<object>.FailureResponse("Trạng thái không hợp lệ."));

            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            if (order == null) return NotFound(ApiResponse<object>.FailureResponse("Không tìm thấy đơn hàng."));

            order.Status = newStatus;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null, $"Cập nhật trạng thái đơn hàng thành: {newStatus}"));
        }
    }
}
