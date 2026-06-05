using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel.Notification;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.DTOs.Common;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Application.Interfaces.IAdminService;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseCrudController<Product>
    {
        private readonly IProductService _productService;
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ProductsController(
            IRepository<Product> repository,
            IProductService productService,
            IReportService reportService,
            IMapper mapper)
            : base(repository)
        {
            _productService = productService;
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 30)
        {
            var products = await _productService.GetAllAsync(pageNumber, pageSize);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos));
        }

        // Ẩn các phương thức kế thừa để tránh Swagger 500
        [NonAction]
        public override Task<IActionResult> GetAll() => base.GetAll();
        [NonAction]
        public override Task<IActionResult> Create(Product entity) => base.Create(entity);
        [NonAction]
        public override Task<IActionResult> Create([FromBody] IEnumerable<Product> entities) => base.Create(entities);
        [NonAction]
        public override Task<IActionResult> Update(Product entity) => base.Update(entity);

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(Guid id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với mã ID: {id}");

            return Ok(ApiResponse<object>.SuccessResponse(null, "Deleted successfully"));
        }

        [HttpGet("export-excel-products")]
        public async Task<IActionResult> ExportProducts()
        {
            var fileBytes = await _reportService.GetProductExcelReportAsync();
            string fileName = $"ProductReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return base.File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("export-pdf-products")]
        [Produces("application/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var fileBytes = await _reportService.GetProductPdfReportAsync("PRODUCT INVENTORY REPORT", "ToyStore System");
            string fileName = $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return base.File(fileBytes, "application/pdf", fileName);
        }
    }
}
