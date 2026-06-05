using Hangfire;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.DTOs.Sales;
using ToyStoreManagement.Domain.Constants;
using ToyStoreManagement.Domain.Entities;
using ToyStoreManagement.Domain.Interfaces;
using ToyStoreManagement.Application.Interfaces;

namespace ToyStoreManagement.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<OrderDetail> _orderDetailRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly INotificationService _notificationService;

        public SaleService(
             IRepository<Product> productRepo,
             IRepository<Order> orderRepo,
             IRepository<OrderDetail> orderDetailRepo,
             IRepository<CartItem> cartItemRepo,
             INotificationService notificationService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _cartItemRepo = cartItemRepo;
            _notificationService = notificationService;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            return await _productRepo.GetQueryable()
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
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            productDto.Id = product.ProductId;
            return productDto;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetProductsAsync();

            var products = await _productRepo.GetQueryable()
                .Where(p => p.Name.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();

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
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null || product.StockQuantity < dto.Quantity)
            {
                throw new Exception("Sản phẩm không tồn tại hoặc số lượng tồn kho không đủ.");
            }

            var existingItem = await _cartItemRepo.GetQueryable()
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
                await _cartItemRepo.AddAsync(cartItem);
            }

            await _cartItemRepo.SaveChangesAsync();
        }

        public async Task<List<CartItemDto>> GetCartAsync(string customerId)
        {
            return await _cartItemRepo.GetQueryable()
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

        public async Task<Guid> CheckoutAsync(OrderRequestDto request)
        {
            try
            {
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    ShippingAddress = request.ShippingAddress,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatuses.Pending,
                    Discount = request.Discount
                };

                decimal totalAmount = 0;

                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);

                    if (product == null)
                        throw new Exception($"Sản phẩm với ID {item.ProductId} không tồn tại.");

                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Sản phẩm '{product.Name}' không đủ hàng (Hiện còn: {product.StockQuantity}).");

                    product.StockQuantity -= item.Quantity;

                    var detail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };

                    await _orderDetailRepo.AddAsync(detail);
                    totalAmount += (detail.Quantity * detail.Price);
                }

                order.TotalAmount = totalAmount;
                await _orderRepo.AddAsync(order);

                await _orderRepo.SaveChangesAsync();

                // 1. SIGNALR: Thông báo Real-time cho Admin
                await _notificationService.SendNotificationAsync($"CÓ ĐƠN HÀNG MỚI: {request.CustomerName} vừa đặt hàng trị giá {totalAmount:N0}đ");

                // 2. HANGFIRE: Đẩy tác vụ gửi Email vào Background Job (không làm khách hàng phải chờ)
                BackgroundJob.Enqueue<IEmailService>(emailService => emailService.SendOrderConfirmationEmailAsync(order.CustomerName, order.OrderId.ToString()));

                return order.OrderId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng.");
            if (order.Status == OrderStatuses.Confirmed) return true;

            bool paymentSuccess = true; 

            if (paymentSuccess)
            {
                order.Status = OrderStatuses.Confirmed;
                await _orderRepo.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            
            // Chỉ cho phép hủy nếu đơn hàng đang ở trạng thái Chờ xác nhận
            if (order == null || order.Status != OrderStatuses.Pending) 
                return false;

            var orderDetails = await _orderDetailRepo.GetQueryable()
                .Where(d => d.OrderId == orderId)
                .ToListAsync();

            foreach (var detail in orderDetails)
            {
                var product = await _productRepo.GetByIdAsync(detail.ProductId);
                if (product != null)
                {
                    // Trả lại kho
                    product.StockQuantity += detail.Quantity;
                    _productRepo.Update(product);
                }
            }

            order.Status = OrderStatuses.Cancelled;
            _orderRepo.Update(order);

            await _orderRepo.SaveChangesAsync();

            return true;
        }

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(Guid customerId)
        {
            var orders = await _orderRepo.GetQueryable()
                .Where(o => o.CustomerId == customerId) // Sửa lỗi logic lấy đơn hàng theo CustomerId
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
