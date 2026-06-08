using System;

namespace ToyStoreManagement.Application.DTOs.Sales
{
    public class ProductSearchDto
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinAge { get; set; } // Tìm đồ chơi phù hợp cho độ tuổi từ X trở lên
        public string? SortBy { get; set; } // "price_asc", "price_desc", "newest"
    }
}
