using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Entities
{
    [Table("INVENTORY_RECEIPT_DETAILS")]
    public class InventoryReceiptDetail
    {
        [Key]
        [Column("INVENTORY_RECEIPT_DETAIL_ID")]
        public Guid InventoryReceiptDetailId { get; set; } = Guid.NewGuid();

        [Column("INVENTORY_RECEIPT_ID")]
        public Guid InventoryReceiptId { get; set; }

        [Column("PRODUCT_ID")]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [Column("UNIT_PRICE", TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        // Quan hệ ngược lại với Phiếu nhập (Tùy chọn nhưng nên có)
        [ForeignKey(nameof(InventoryReceiptId))]
        public virtual InventoryReceipt InventoryReceipt { get; set; } = null!;
    }
}
