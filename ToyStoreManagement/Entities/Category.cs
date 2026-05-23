using ToyStoreManagement.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Entities
{
    [Table("CATEGORY")] // Tên bảng trong DB
    public class Category
    {
        [Key]
        [Column("CATEGORY_ID")] // Chỉ định rõ tên cột trong DB
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        [Column("CATEGORY_NAME")]
        public string CategoryName { get; set; } = string.Empty;

        // Navigation property: Một danh mục có nhiều sản phẩm
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
