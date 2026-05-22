using ToyStoreManagement.DTOs;
using ToyStoreManagement.DTOs.Sales;

namespace ToyStoreManagement.IServices
{
    public interface ISaleService
    {
        // Product CRUD
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        //Task<List<ProductDto>> GetProductsByAgeAsync(int age);
        Task<List<ProductDto>> SearchProductsAsync(string keyword); // <-- Mới: Tìm kiếm

        // Cart Logic
        Task AddToCartAsync(AddToCartDto dto);
        Task<List<CartItemDto>> GetCartAsync(string customerId);
        //Task ClearCartAsync(string customerId);

        // Order & Payment Logic
        //Task<OrderDto> CheckoutAsync(string customerId, CheckoutDto dto);
        Task<Guid> CheckoutAsync(OrderRequestDto orderRequestDto);
        Task<bool> ProcessPaymentAsync(Guid orderId);
        Task<bool> CancelOrderAsync(Guid orderId); // <-- Mới: Hủy đơn hàng
        Task<List<OrderDto>> GetCustomerOrdersAsync(Guid customerId);
    }
}
