using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Domain.Entities;
using System.Linq;

namespace ToyStoreManagement.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext
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
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public DbSet<DiscountCode> DiscountCodes { get; set; } = null!;
        public DbSet<StoreSetting> StoreSettings { get; set; } = null!;

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

            // 2. TỰ ĐỘNG HÓA TÊN BẢNG VÀ SCHEMA
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName()?.ToUpper();

                if (tableName != null)
                {
                    entity.SetSchema("C##ORACLEDB2026");
                    entity.SetTableName(tableName);
                }
            }

            // Xử lý kiểu BOOLEAN cho Oracle
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetColumnType("NUMBER(1)");
                    }
                }
            }
        }
    }
}
