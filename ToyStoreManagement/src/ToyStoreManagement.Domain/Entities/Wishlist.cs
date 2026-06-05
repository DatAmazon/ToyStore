using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Domain.Entities
{
    [Table("WISHLIST")]
    public class Wishlist
    {
        [Key]
        [Column("WISHLIST_ID")]
        public Guid WishlistId { get; set; } = Guid.NewGuid();

        [Required]
        [Column("USER_ID")]
        public string UserId { get; set; } = string.Empty; // Identity User ID

        [Required]
        [Column("PRODUCT_ID")]
        public Guid ProductId { get; set; }

        [Column("ADDED_DATE")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
}
