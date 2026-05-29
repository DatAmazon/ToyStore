using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Entities;

namespace ToyStoreManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Product> Products { get; set; } = null!;   
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<InventoryReceipt> InventoryReceipts { get; set; } = null!;
        public DbSet<InventoryReceiptDetail> InventoryReceiptDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Oracle yêu cầu cấu hình chính xác cho kiểu decimal
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("NUMBER(18, 2)");
            }

            // 2. TỰ ĐỘNG HÓA TÊN BẢNG VÀ SCHEMA (Cho 100 hay 1000 bảng đều được)
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Lấy tên Class (ví dụ Category) chuyển thành VIẾT HOA (CATEGORIES)
                var tableName = entity.GetTableName().ToUpper();

                // Gán lại tên bảng đã viết hoa và Schema C##ORACLEDB2026
                entity.SetSchema("C##ORACLEDB2026");
                entity.SetTableName(tableName);
            }
        }
    }
}
