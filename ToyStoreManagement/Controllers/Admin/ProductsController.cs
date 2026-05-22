using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductStoreManagement.Entities;
using ToyStoreManagement.Controllers.Base;
using ToyStoreManagement.Data;
using ToyStoreManagement.IRepositories;

namespace ToyStoreManagement.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseCrudController<Product>
    {
        private readonly AppDbContext _context;

        public ProductsController(IRepository<Product> repository, AppDbContext context)
            : base(repository)
        {
            _context = context;
        }

        // 1. Lấy tất cả: Ghi đè để Include thêm Category cho Frontend hiển thị tên loại
        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return Ok(products);
        }

        // 2. Lấy theo ID: Ghi đè để lấy chi tiết sản phẩm kèm Category
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetById(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound("Sản phẩm không tồn tại.");
            return Ok(product);
        }

        // 3. Tạo mới: Ghi đè để kiểm tra logic kinh doanh (Giá, Tuổi...)
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
                return BadRequest("Tên sản phẩm không được để trống.");

            if (product.Price < 0)
                return BadRequest("Giá sản phẩm không thể âm.");

            // Gọi base để lưu vào DB
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        // 4. Cập nhật: Ghi đè để kiểm tra sự tồn tại và logic trước khi Update
        [HttpPut]
        public override async Task<IActionResult> Update([FromBody] Product product)
        {
            var exists = await _repository.GetByIdAsync(product.ProductId);
            if (exists == null) return NotFound("Sản phẩm cần cập nhật không tồn tại.");

            // Bạn có thể giữ lại số lượng kho cũ nếu không muốn API này làm thay đổi kho trái phép
            // product.StockQuantity = exists.StockQuantity;

            _repository.Update(product);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        // 5. Xóa: Ghi đè để kiểm tra xem sản phẩm có đang nằm trong đơn hàng nào không
        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();

            // Logic kiểm tra ràng buộc (nếu cần):
            // var hasOrders = await _context.OrderDetails.AnyAsync(x => x.ProductId == id);
            // if (hasOrders) return BadRequest("Không thể xóa sản phẩm đã có trong đơn hàng.");

            _repository.Delete(product);
            await _repository.SaveChangesAsync();
            return NoContent();
        }

        //private readonly AppDbContext _context;

        //public ProductsController(AppDbContext context) => _context = context;

        //[HttpGet]
        //public async Task<List<Product>> GetAll(){
        //    return await _context.Products.ToListAsync();
        //} 

        //[HttpPost]
        //public async Task<IActionResult> Create(Product product)
        //{
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetAll), new { id = product.ProductId }, product);
        //}
    }
}
