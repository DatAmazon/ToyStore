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
        private readonly ICartService _cartService;
        private readonly IDiscountService _discountService;

        public SaleService(
             IRepository<Product> productRepo,
             IRepository<Order> orderRepo,
             IRepository<OrderDetail> orderDetailRepo,
             IRepository<CartItem> cartItemRepo,
             INotificationService notificationService,
             ICartService cartService,
             IDiscountService discountService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _cartItemRepo = cartItemRepo;
            _notificationService = notificationService;
            _cartService = cartService;
            _discountService = discountService;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            return await _productRepo.GetQueryable()
                .Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity
                }).ToListAsync();
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                DiscountPrice = productDto.DiscountPrice,
                StockQuantity = productDto.StockQuantity
            };
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            productDto.Id = product.ProductId;
            return productDto;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto)
        {
            var query = _productRepo.GetQueryable().Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(keyword));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => (p.DiscountPrice ?? p.Price) >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => (p.DiscountPrice ?? p.Price) <= searchDto.MaxPrice.Value);
            }

            if (searchDto.MinAge.HasValue)
            {
                query = query.Where(p => p.MinimumAge >= searchDto.MinAge.Value);
            }

            switch (searchDto.SortBy?.ToLower())
            {
                case "price_asc":
                    query = query.OrderBy(p => p.DiscountPrice ?? p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.DiscountPrice ?? p.Price);
                    break;
                case "newest":
                default:
                    query = query.OrderBy(p => p.Name); // Sắp xếp theo tên từ A -> Z theo yêu cầu
                    break;
            }

            var products = await query.ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category?.CategoryName ?? "N/A"
            }).ToList();
        }

        public async Task<Guid> CheckoutAsync(OrderRequestDto request)
        {
            try
            {
                DiscountCode? validDiscount = null;
                if (!string.IsNullOrWhiteSpace(request.DiscountCode))
                {
                    var validationResult = await _discountService.ValidateDiscountAsync(request.DiscountCode);
                    if (!validationResult.Success)
                    {
                        throw new Exception(validationResult.Message);
                    }
                    validDiscount = validationResult.Discount;
                }

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    ShippingAddress = request.ShippingAddress,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatuses.Pending,
                    Discount = 0
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
                        Price = product.DiscountPrice ?? product.Price
                    };

                    await _orderDetailRepo.AddAsync(detail);
                    totalAmount += (detail.Quantity * detail.Price);
                }

                decimal discountAmount = 0;
                if (validDiscount != null)
                {
                    if (validDiscount.DiscountType == ToyStoreManagement.Domain.Enums.AppEnums.DiscountType.Percentage)
                    {
                        discountAmount = totalAmount * (validDiscount.DiscountValue / 100);
                    }
                    else
                    {
                        discountAmount = validDiscount.DiscountValue;
                    }
                    await _discountService.ApplyDiscountAsync(validDiscount.Id);
                }

                order.TotalAmount = totalAmount;
                order.Discount = discountAmount;
                order.FinalAmount = totalAmount - discountAmount;
                if (order.FinalAmount < 0) order.FinalAmount = 0;

                await _orderRepo.AddAsync(order);
                await _orderRepo.SaveChangesAsync();

                // Nếu là khách hàng đã đăng nhập, làm sạch giỏ hàng trong DB
                if (request.CustomerId.HasValue)
                {
                    // Lấy userId dưới dạng string từ Identity để xóa giỏ hàng
                    var userIdStr = request.CustomerId.Value.ToString();
                    await _cartService.ClearCartAsync(userIdStr);
                }

                // 1. SIGNALR: Thông báo Real-time cho Admin
                await _notificationService.SendNotificationAsync($"CÓ ĐƠN HÀNG MỚI: {request.CustomerName} vừa đặt hàng trị giá {order.FinalAmount:N0}đ");

                // 2. HANGFIRE: Đẩy tác vụ gửi Email vào Background Job (không làm khách hàng phải chờ)
                BackgroundJob.Enqueue<IEmailService>(emailService => emailService.SendOrderConfirmationEmailAsync(order.CustomerName, order.OrderId.ToString()));

                return order.OrderId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderResponseDto> GetOrderDetailsAsync(Guid orderId)
        {
            var order = await _orderRepo.GetQueryable()
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Discount = order.Discount,
                FinalAmount = order.FinalAmount,
                Details = order.Details.Select(d => new OrderDetailResponseDto
                {
                    ProductName = d.Product?.Name ?? "N/A",
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };
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

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId)
        {
            // Tìm CustomerId Guid từ Identity UserId string nếu cần, 
            // nhưng ở bước Register tôi đã dùng Email làm UserName và IdentityId cho Customer.
            // Để đơn giản, tôi sẽ tìm Customer có Email tương ứng hoặc mapping.
            // Giả sử customerId truyền vào là string IdentityId.
            
            var orders = await _orderRepo.GetQueryable()
                .Where(o => o.CustomerId.ToString() == customerId) 
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    TotalAmount = o.TotalAmount,
                    Discount = o.Discount,
                    FinalAmount = o.FinalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            // Nếu chuyển sang trạng thái Hủy, cần trả lại hàng vào kho
            if (newStatus == OrderStatuses.Cancelled && order.Status != OrderStatuses.Cancelled)
            {
                var details = await _orderDetailRepo.GetQueryable()
                    .Where(d => d.OrderId == orderId)
                    .ToListAsync();

                foreach (var item in details)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _productRepo.Update(product);
                    }
                }
            }

            order.Status = newStatus;
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            // Gửi thông báo SignalR (Tùy chọn)
            await _notificationService.SendNotificationAsync($"Đơn hàng {order.OrderId} đã chuyển sang trạng thái: {newStatus}");

            return true;
        }
    }
}
