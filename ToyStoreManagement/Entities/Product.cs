using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Entities
{
    [Table("PRODUCT")]
    public class Product
    {
        [Key]
        [Column("PRODUCT_ID")]
        public Guid ProductId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;

        [Column("PRICE", TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Column("STOCK_QUANTITY")]
        public int StockQuantity { get; set; }

        // --- Các thuộc tính đặc thù phù hợp từ Toy đưa sang ---
        [Column("MINIMUM_AGE")]
        public int MinimumAge { get; set; } // Độ tuổi tối thiểu

        [StringLength(150)]
        [Column("MANUFACTURER")]
        public string Manufacturer { get; set; } = string.Empty; // Nhà sản xuất

        // --- Quan hệ với Danh mục ---
        [Column("CATEGORY_ID")]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }
    }
}
