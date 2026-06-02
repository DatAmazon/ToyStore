using System;
using System.ComponentModel.DataAnnotations;

namespace ToyStoreManagement.Domain.Entities
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;
    }
}
