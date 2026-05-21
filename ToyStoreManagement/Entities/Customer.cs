using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Entities
{
    [Table("CUSTOMER")]
    public class Customer
    {
        [Key]
        [Column("CUSTOMER_ID")]
        public Guid CustomerId { get; set; } = Guid.NewGuid();

        [Required]
        [Column("FULL_NAME")]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Column("PHONE")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("EMAIL")]
        [StringLength(255)]
        public string? Email { get; set; }

        [Column("ADDRESS")]
        public string? Address { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Quan hệ: Một khách hàng có thể có nhiều đơn hàng
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
