using System;
using System.ComponentModel.DataAnnotations;
using ToyStoreManagement.Domain.Enums;

namespace ToyStoreManagement.Domain.Entities
{
    public class DiscountCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public decimal DiscountValue { get; set; }

        [Required]
        public AppEnums.DiscountType DiscountType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
    }
}
