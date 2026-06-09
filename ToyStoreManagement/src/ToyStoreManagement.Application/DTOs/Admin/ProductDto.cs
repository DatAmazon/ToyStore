using Microsoft.AspNetCore.Http;
using System;

namespace ToyStoreManagement.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountPercentage { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? Reviews { get; set; }
        public string? Badge { get; set; }
        public string? BadgeColor { get; set; }
    }

    public class ProductExportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int MinimumAge { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? Reviews { get; set; }
        public string? Badge { get; set; }
        public string? BadgeColor { get; set; }
    }

    public class ProductUpdateDto
    {
        // Khớp với trường "id" dạng chuỗi Guid từ FE
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int? DiscountPercentage { get; set; }

        // Khớp với trường "stockQuantity" từ FE
        public int StockQuantity { get; set; }

        // BẮT BUỘC: Thay vì nhận categoryName (chuỗi chữ), FE phải truyền sang ID của danh mục
        public Guid CategoryId { get; set; }

        // Khớp hoàn toàn với thuộc tính "image" (binary) từ FE
        public IFormFile? Image { get; set; }
    }
}
