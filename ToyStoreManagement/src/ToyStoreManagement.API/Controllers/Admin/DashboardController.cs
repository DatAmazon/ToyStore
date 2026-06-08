using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Domain.Constants;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var today = DateTime.UtcNow.Date;

            // Doanh thu hôm nay (Chỉ tính đơn đã xác nhận)
            var todayRevenue = await _unitOfWork.Repository<Order>().GetQueryable()
                .Where(o => o.OrderDate >= today && o.Status == OrderStatuses.Confirmed)
                .SumAsync(o => o.FinalAmount);

            // Số đơn hàng mới (Pending)
            var newOrdersCount = await _unitOfWork.Repository<Order>().GetQueryable()
                .CountAsync(o => o.Status == OrderStatuses.Pending);

            // Tổng số khách hàng
            var totalCustomers = await _unitOfWork.Repository<Customer>().GetQueryable().CountAsync();

            // Sản phẩm sắp hết hàng (Low stock: <= 5)
            var lowStockProducts = await _unitOfWork.Repository<Product>().GetQueryable()
                .Where(p => p.StockQuantity <= 5)
                .Select(p => new { p.ProductId, p.Name, p.StockQuantity })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                TodayRevenue = todayRevenue,
                NewOrders = newOrdersCount,
                TotalCustomers = totalCustomers,
                LowStockAlerts = lowStockProducts
            }));
        }
    }
}
