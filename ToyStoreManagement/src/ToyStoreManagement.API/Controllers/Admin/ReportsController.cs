using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Domain.Constants;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("inventory/excel")]
        public async Task<IActionResult> ExportInventoryExcel()
        {
            try
            {
                var fileBytes = await _reportService.GetProductExcelReportAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }

        [HttpGet("inventory/pdf")]
        public async Task<IActionResult> ExportInventoryPdf()
        {
            try
            {
                var fileBytes = await _reportService.GetProductPdfReportAsync("BÁO CÁO TỒN KHO", "Admin");
                return File(fileBytes, "application/pdf", $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }

        [HttpGet("orders/excel")]
        public async Task<IActionResult> ExportOrdersExcel([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var fileBytes = await _reportService.GetOrderExcelReportAsync(fromDate, toDate);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"OrdersReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }

        [HttpGet("invoice/{id}/pdf")]
        public async Task<IActionResult> ExportInvoicePdf(Guid id)
        {
            try
            {
                var fileBytes = await _reportService.GetInvoicePdfAsync(id);
                return File(fileBytes, "application/pdf", $"Invoice_{id}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất hóa đơn: {ex.Message}");
            }
        }
    }
}
