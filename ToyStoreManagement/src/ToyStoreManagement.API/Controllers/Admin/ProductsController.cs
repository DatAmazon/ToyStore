using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Application.Services.AdminService;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseCrudController<Product>
    {
        private readonly ProductService _productService;
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _env;

        public ProductsController(
            IRepository<Product> repository,
            ProductService productService,
            IReportService reportService,
            IWebHostEnvironment env)
            : base(repository)
        {
            _productService = productService;
            _reportService = reportService;
            _env = env;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                MinimumAge = p.MinimumAge,
                Manufacturer = p.Manufacturer,
                CategoryName = p.Category?.CategoryName ?? "N/A"
            });
            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound("Product not found");

            var productDto = new ProductDto
            {
                Id = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                MinimumAge = p.MinimumAge,
                Manufacturer = p.Manufacturer,
                CategoryName = p.Category?.CategoryName ?? "N/A"
            };
            return Ok(productDto);
        }

        // ... các hàm Create, Update, Delete giữ nguyên hoặc sửa tương tự nếu cần ...

        [HttpPost("create-product")]
        public override async Task<IActionResult> Create([FromBody] Product product)
        {
            var result = await _productService.CreateAsync(product);
            if (!result.Success) return BadRequest(result.Message);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProductId }, result.Data);
        }

        [HttpPost("create-products")]
        public async Task<IActionResult> CreateMultiple([FromBody] IEnumerable<Product> products)
        {
            try
            {
                var result = await _productService.CreateWithAutoCategoryAsync(products);
                return result.Success ? Ok(result.Message) : BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public override async Task<IActionResult> Update([FromBody] Product product)
        {
            var success = await _productService.UpdateAsync(product);
            return success ? NoContent() : NotFound("Product not found");
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(Guid id)
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("export-excel-products")]
        public async Task<IActionResult> ExportProducts()
        {
            try
            {
                string templatePath = Path.Combine(_env.WebRootPath, "templates", "Report", "ProductReport.xlsx");
                var fileBytes = await _reportService.ExportProductsToExcelAsync(templatePath);
                string fileName = $"ProductReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return base.File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            catch (Exception ex)
            {
                return BadRequest($"Error exporting Excel: {ex.Message}");
            }
        }

        [HttpGet("export-pdf-products")]
        [Produces("application/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                string templatePath = Path.Combine(_env.WebRootPath, "templates", "Print", "ProductPrint.docx");
                var fileBytes = await _reportService.ExportProductsToPdfAsync(templatePath, "PRODUCT INVENTORY REPORT", "ToyStore System");

                string fileName = $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                //Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");

                return base.File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting PDF: {ex.Message}");
            }
        }
    }
}
