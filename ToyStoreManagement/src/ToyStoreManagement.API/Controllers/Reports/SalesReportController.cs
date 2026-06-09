using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public SalesReportController(IReportService reportService) => _reportService = reportService;

        [HttpGet("excel")]
        public async Task<IActionResult> ExportSalesReportExcel([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var fileBytes = await _reportService.GetSalesReportExcelAsync(fromDate, toDate);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SalesReport_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }
    }
}
