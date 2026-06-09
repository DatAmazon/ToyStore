using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public InventoryReportController(IReportService reportService) => _reportService = reportService;

        [HttpGet("excel")]
        public async Task<IActionResult> ExportInventoryExcel()
        {
            try
            {
                var fileBytes = await _reportService.GetProductExcelReportAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"InventoryReport_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportInventoryPdf()
        {
            try
            {
                var fileBytes = await _reportService.GetProductPdfReportAsync("BÁO CÁO TỒN KHO", "Admin");
                return File(fileBytes, "application/pdf", $"InventoryReport_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }
    }
}
