namespace ToyStoreManagement.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = "Khách vãng lai";
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
