using Microsoft.AspNetCore.Mvc;
using ProductStoreManagement.Entities;
using ToyStoreManagement.DTOs.Admin;
using ToyStoreManagement.IRepositories;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToysController : ControllerBase
    {
        private readonly IToyRepository _toyRepository;

        public ToysController(IToyRepository toyRepository)
        {
            _toyRepository = toyRepository;
        }

        // GET: api/Toys
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _toyRepository.GetAllAsync());

        // GET: api/Toys/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var toy = await _toyRepository.GetByIdAsync(id);
            return toy == null ? NotFound("Không tìm thấy đồ chơi này.") : Ok(toy);
        }

        // POST: api/Toys (Tạo mới mặt hàng đồ chơi)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToyCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                MinimumAge = dto.MinimumAge,
                Manufacturer = dto.Manufacturer,
                StockQuantity = 0 // Mới khai báo, chưa nhập kho thì tồn kho bằng 0
            };

            await _toyRepository.AddAsync(product);
            await _toyRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        // PUT: api/Toys/5 (Cập nhật thông tin đồ chơi)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.ProductId) return BadRequest("ID không đồng nhất.");

            _toyRepository.Update(updatedProduct);
            await _toyRepository.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Toys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var toy = await _toyRepository.GetByIdAsync(id);
            if (toy == null) return NotFound();

            _toyRepository.Delete(toy);
            await _toyRepository.SaveChangesAsync();
            return Ok("Xóa thành công.");
        }

        // GET: api/Toys/low-stock (Lấy đồ chơi sắp hết hàng dưới 5 món để cảnh báo)
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock() => Ok(await _toyRepository.GetLowStockToysAsync(5));

    }
}
