namespace ToyStoreManagement.Application.DTOs.Sales
{
    public class OrderRequestDto
    {
        //Order Request
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public List<OrderRequestItemDto> Items { get; set; } = new List<OrderRequestItemDto>();
    }

    public class OrderRequestItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    //Order Response dùng để hiển thị chi tiết khi xem lại đơn hàng
    public class OrderDetailResponseDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal => Quantity * Price;
    }

    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public List<OrderDetailResponseDto> Details { get; set; } = new List<OrderDetailResponseDto>();
    }
}
