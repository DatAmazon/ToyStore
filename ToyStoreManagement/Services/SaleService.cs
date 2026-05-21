using Microsoft.EntityFrameworkCore;
using ProductStoreManagement.Entities;
using ToyStoreManagement.Data;
using ToyStoreManagement.DTOs;
using ToyStoreManagement.Entities;
using ToyStoreManagement.IRepositories;
using ToyStoreManagement.IServices;

namespace ToyStoreManagement.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Order> _orderRepo;


        // Dependency Injection nạp DbContext vào Service
        public SaleService(
             AppDbContext context,
             IRepository<Order> orderRepo)
        {
            _context = context;
            _orderRepo = orderRepo;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                }).ToListAsync();
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.ProductId;
            return productDto;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetProductsAsync();

            var products = await _context.Products.Where(p =>
                p.Name.ToLower().Contains(keyword.ToLower())).ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();
        }

        public async Task AddToCartAsync(AddToCartDto dto)
        {
            // Kiểm tra hàng tồn kho trước khi cho vào giỏ
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null || product.StockQuantity < dto.Quantity)
            {
                throw new Exception("Sản phẩm không tồn tại hoặc số lượng tồn kho không đủ.");
            }

            // Nếu sản phẩm đã có trong giỏ của khách đó, ta cộng dồn số lượng
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId && c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CustomerId = dto.CustomerId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItemDto>> GetCartAsync(string customerId)
        {
            return await _context.CartItems
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CartItemDto
                {
                    Id = c.CartItemId,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    Quantity = c.Quantity,
                    Price = c.Product.Price
                }).ToListAsync();
        }

        public async Task<OrderDto> CheckoutAsync(string customerId, CheckoutDto dto)
        {
            // 1. Lấy toàn bộ hàng trong giỏ ra
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (!cartItems.Any()) throw new Exception("Giỏ hàng trống rỗng, không thể đặt hàng.");

            // Dùng Database Transaction để đảm bảo tính toàn vẹn dữ liệu (Atomicity)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerName = dto.CustomerName,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending"
                };
                _context.Orders.Add(order);

                decimal totalAmount = 0;

                foreach (var item in cartItems)
                {
                    // 2. Kiểm tra và trừ kho (Concurrency check cơ bản)
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        throw new Exception($"Sản phẩm {item.Product.Name} đã hết hàng hoặc không đủ số lượng.");
                    }

                    item.Product.StockQuantity -= item.Quantity; // Trừ kho trực tiếp

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    };

                    totalAmount += item.Quantity * item.Product.Price;
                    _context.OrderDetails.Add(orderDetail);
                }

                order.TotalAmount = totalAmount;

                // 3. Xóa giỏ hàng sau khi đã chuyển thành hóa đơn đặt hàng
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Xác nhận lưu mọi thay đổi xuống Oracle DB

                return new OrderDto
                {
                    OrderId = order.OrderId,
                    CustomerName = order.CustomerName,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); // Nếu lỗi bất kỳ bước nào, hoàn tác lại toàn bộ dữ liệu sạch sẽ
                throw;
            }
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng.");
            if (order.Status == "Paid") return true;

            // Giả lập logic gọi cổng thanh toán (VNPAY/Momo)...)
            bool paymentSuccess = true; // Giả sử cổng thanh toán báo thành công

            if (paymentSuccess)
            {
                order.Status = "Paid";
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            // Chỉ cho phép hủy nếu đơn hàng đang ở trạng thái 'Pending'
            if (order == null || order.Status != "Pending") return false;

            // 1. Lấy danh sách chi tiết đơn hàng để biết cần trả lại những gì vào kho
            var orderDetails = await _context.OrderDetails
                .Where(d => d.OrderId == orderId)
                .ToListAsync();

            foreach (var detail in orderDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    product.StockQuantity += detail.Quantity; // Hoàn kho
                    _context.Products.Update(product);
                }
            }

            // 2. Cập nhật trạng thái đơn hàng
            order.Status = "Cancelled";
            _context.Orders.Update(order);

            // 3. Lưu toàn bộ thay đổi xuống Oracle DB
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(Guid customerId)
        {
            // Query the Orders DbSet with a predicate rather than using FindAsync (which expects key values)
            var orders = await _context.Orders
                .Where(o => o.OrderId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            return orders;
        }
    }
}