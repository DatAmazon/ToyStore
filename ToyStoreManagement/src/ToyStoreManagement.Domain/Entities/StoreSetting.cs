using System;
using System.ComponentModel.DataAnnotations;

namespace ToyStoreManagement.Domain.Entities
{
    public class StoreSetting
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
