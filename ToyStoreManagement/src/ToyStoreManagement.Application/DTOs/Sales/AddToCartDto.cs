using ToyStoreManagement.Domain.Constants;

namespace ToyStoreManagement.Application.DTOs.Sales
{
    public class AddToCartDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = CartItemStatuses.Active;
    }

    public class UpdateCartItemDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
