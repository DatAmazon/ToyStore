using ToyStoreManagement.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Entities
{
    [Table("CART_ITEM")]
    public class CartItem
    {
        [Key]
        [Column("CART_ITEM_ID")]
        public Guid CartItemId { get; set; } = Guid.NewGuid();

        [Column("CUSTOMER_ID")]
        public string CustomerId { get; set; } = "Khách vãng lai";

        [Column("PRODUCT_ID")]
        public Guid ProductId { get; set; }

        [Column("QUANTITY")]
        public int Quantity { get; set; }

        [StringLength(20)]
        //Active (Đang chọn), Saved (Lưu để mua sau), Converted (Đã chuyển thành đơn hàng)
        public string Status { get; set; } = "Active";

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
