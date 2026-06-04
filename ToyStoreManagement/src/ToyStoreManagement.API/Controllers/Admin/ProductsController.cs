using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ToyStoreManagement.Application.DTOs;
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
        public override async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound("Product not found");

            var productDto = _mapper.Map<ProductDto>(p);
            return Ok(productDto);
        }

        // ... các hàm Create, Update, Delete giữ nguyên hoặc sửa tương tự nếu cần ...

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromForm] Product product, IFormFile? file)
        {
            var result = await _productService.CreateAsync(product, file);
            if (!result.Success) return BadRequest(result.Message);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProductId }, result.Data);
        }

        [HttpPost("create-products")]
        public async Task<IActionResult> CreateMultipleProducts([FromBody] IEnumerable<Product> products)
        {
            var result = await _productService.CreateWithAutoCategoryAsync(products);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateDto product, IFormFile? file)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Kiểm tra xem sản phẩm có tồn tại trong hệ thống không
            var existingProduct = await _productService.GetByIdAsync(product.Id);
            if (existingProduct == null) return NotFound("Product not found");

            // 2. Sử dụng AutoMapper để cập nhật đè các trường thay đổi từ DTO vào Entity hiện tại
            // Điều này giúp giữ nguyên các giá trị cũ trong DB không có trên form (như Rating, Reviews, Badge...)
            _mapper.Map(product, existingProduct);

            // 3. Đẩy Entity đã cập nhật cùng file ảnh xuống Service để xử lý lưu MinIO và Oracle DB
            var success = await _productService.UpdateAsync(existingProduct, product.Image);

            return success ? NoContent() : BadRequest("Đã có lỗi xảy ra trong quá trình cập nhật sản phẩm.");
        }

        // Ẩn các phương thức kế thừa để tránh Swagger 500
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
            return success ? NoContent() : NotFound();
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
