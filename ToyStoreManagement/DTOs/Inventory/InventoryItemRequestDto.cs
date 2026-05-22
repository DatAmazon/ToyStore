namespace ToyStoreManagement.DTOs.Inventory
{
    public class InventoryItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Giá vốn nhập vào
    }

    public class InventoryRequest
    {
        public Guid SupplierId { get; set; }
        public List<InventoryItemRequestDto> Items { get; set; } = new List<InventoryItemRequestDto>();
    }
}
