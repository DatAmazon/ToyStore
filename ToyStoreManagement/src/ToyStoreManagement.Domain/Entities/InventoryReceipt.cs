using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Domain.Entities
{
    [Table("INVENTORY_RECEIPTS")]
    public class InventoryReceipt
    {
        [Key]
        [Column("INVENTORY_RECEIPT_ID")]
        public Guid InventoryReceiptId { get; set; } = Guid.NewGuid();

        [Column("RECEIVED_DATE")]
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;

        [Column("SUPPLIER_ID")]
        public Guid SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))] // Dùng nameof để tránh gõ sai tên biến
        public virtual Supplier Supplier { get; set; } = null!;

        [Column("TOTAL_AMOUNT", TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        // Navigation property: Một phiếu nhập có nhiều chi tiết
        public virtual ICollection<InventoryReceiptDetail> Details { get; set; } = new List<InventoryReceiptDetail>();
    }
}
