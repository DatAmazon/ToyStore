using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToyStoreManagement.Domain.Enums;

namespace ToyStoreManagement.Domain.Entities
{
    [Table("ORDERS")]
    public class Order
    {
        [Key]
        [Column("ORDER_ID")]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        [Column("ORDER_DATE")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column("TOTAL_AMOUNT", TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column("DISCOUNT", TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; }

        [Column("FINAL_AMOUNT", TypeName = "decimal(18, 2)")]
        public decimal FinalAmount { get; set; }

        [Column("CUSTOMER_ID")]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!; // Thêm virtual để hỗ trợ Lazy Loading

        [Required]
        [StringLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(100)]
        [Column("CUSTOMER_NAME")]
        public string CustomerName { get; set; } = "Khách vãng lai";

        [StringLength(20)]
        [Column("CUSTOMER_PHONE")]
        public string CustomerPhone { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("SHIPPING_ADDRESS")]
        public string ShippingAddress { get; set; } = string.Empty;

        public virtual ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
}
