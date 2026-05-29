using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Entities;
using ToyStoreManagement.IRepositories;
using ToyStoreManagement.Services.AdminService;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseCrudController<Product>
    {
        private readonly ProductsService _productsService;

        public ProductsController(IRepository<Product> repository, ProductsService productsService)
            : base(repository)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public override async Task<IActionResult> GetAll() => Ok(await _productsService.GetAllAsync());

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productsService.GetByIdAsync(id);
            return product == null ? NotFound("Không tìm thấy") : Ok(product);
        }

        [HttpPost("create-product")]
        public override async Task<IActionResult> Create([FromBody] Product product)
        {
            var result = await _productsService.CreateAsync(product);
            if (!result.Success) return BadRequest(result.Message);
            return CreatedAtAction(nameof(GetById), new { id = result.Data.ProductId }, result.Data);
        }

        [HttpPost("create-products")]
        public async Task<IActionResult> CreateMultiple([FromBody] IEnumerable<Product> products)
        {
            try
            {
                var result = await _productsService.CreateWithAutoCategoryAsync(products);
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
            var success = await _productsService.UpdateAsync(product);
            return success ? NoContent() : NotFound("Không thấy sản phẩm");
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(Guid id)
        {
            var success = await _productsService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("export-excel-products")]
        public async Task<IActionResult> ExportProducts()
        {
            try
            {
                var fileBytes = await _productsService.ExportProductsToExcelAsync();
                var fileName = $"BaoCaoHangHoa_{DateTime.Now:ddMMyyyy_HHmm}.xlsx";

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi xuất Excel: {ex.Message}");
            }
        }

    }
}
