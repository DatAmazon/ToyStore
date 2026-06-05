using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Domain.Entities
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

        [Column("IMAGE_URL")]
        public string? ImageUrl { get; set; }

        [Column("RATING")]
        public double? Rating { get; set; }

        [Column("REVIEWS")]
        public int? Reviews { get; set; }

        [StringLength(50)]
        [Column("BADGE")]
        public string? Badge { get; set; }

        [StringLength(50)]
        [Column("BADGE_COLOR")]
        public string? BadgeColor { get; set; }

        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        [Column("SEARCH_NAME")]
        public string? SearchName { get; set; }

        [Column("IS_DELETED")]
        public bool IsDeleted { get; set; } = false;
    }
}
