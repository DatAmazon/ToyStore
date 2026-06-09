using System;

namespace ToyStoreManagement.Application.DTOs.Admin
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountPercentage { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Badge { get; set; }
        public string? BadgeColor { get; set; }
    }
}
