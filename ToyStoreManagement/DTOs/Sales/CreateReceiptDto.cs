namespace ToyStoreManagement.DTOs.Sales
{
    public class CreateReceiptDto
    {
        public Guid SupplierId { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new();
    }
}
