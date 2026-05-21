namespace ToyStoreManagement.DTOs
{
    public class AddToCartDto
    {
        public string CustomerId { get; set; } = "Khách vãng lai";
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
