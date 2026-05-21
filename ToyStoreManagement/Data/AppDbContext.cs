using Microsoft.EntityFrameworkCore;
using ProductStoreManagement.Entities;
using ToyStoreManagement.Entities;

namespace ToyStoreManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;   
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Oracle viết hoa toàn bộ tên bảng và cột theo chuẩn mặc định, 
            // Ta có thể cấu hình thêm fluent API nếu cần thiết tại đây.
        }
    }
}
