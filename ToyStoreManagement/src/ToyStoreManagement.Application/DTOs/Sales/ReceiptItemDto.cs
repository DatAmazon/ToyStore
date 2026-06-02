namespace ToyStoreManagement.Application.DTOs.Sales
{
    public class ReceiptItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
