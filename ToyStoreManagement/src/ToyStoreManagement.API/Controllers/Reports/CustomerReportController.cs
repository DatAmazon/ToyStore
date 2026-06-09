using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public CustomerReportController(IReportService reportService) => _reportService = reportService;

        [HttpGet("excel")]
        public async Task<IActionResult> ExportCustomerReportExcel()
        {
            try
            {
                var fileBytes = await _reportService.GetCustomerReportExcelAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"CustomerReport_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất file: {ex.Message}");
            }
        }
    }
}
