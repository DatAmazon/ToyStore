using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Domain.Entities
{
    [Table("ORDER_DETAILS")]
    public class OrderDetail
    {
        [Key]
        [Column("ORDER_DETAIL_ID")]
        public Guid OrderDetailId { get; set; } = Guid.NewGuid();

        [Column("ORDER_ID")]
        public Guid OrderId { get; set; }

        [Column("PRODUCT_ID")]
        public Guid ProductId { get; set; }

        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [Column("PRICE", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal SubTotal => Quantity * Price;

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
}
