using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.DTOs.Sales;

namespace ToyStoreManagement.Application.Interfaces
{
    public interface ISaleService
    {
        // Product CRUD
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        //Task<List<ProductDto>> GetProductsByAgeAsync(int age);
        Task<List<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto); // <-- Mới: Tìm kiếm nâng cao

        // Order & Payment Logic
        //Task<OrderDto> CheckoutAsync(string customerId, CheckoutDto dto);
        Task<Guid> CheckoutAsync(OrderRequestDto orderRequestDto);
        Task<OrderResponseDto> GetOrderDetailsAsync(Guid orderId);
        Task<bool> ProcessPaymentAsync(Guid orderId);
        Task<bool> CancelOrderAsync(Guid orderId); // <-- Mới: Hủy đơn hàng
        Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus);
    }
}
