using ProductStoreManagement.Entities;

namespace ToyStoreManagement.Services
{
    public class InventoryReceiptDetail1
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InventoryReceiptId { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
