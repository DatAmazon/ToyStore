using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToyStoreManagement.Domain.Entities;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();
            
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
            {
                var auditEntry = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = System.DateTime.UtcNow,
                    UserId = "System" // Nếu có HttpContextAccessor, bạn có thể lấy tên User thực tế ở đây
                };

                var keyValues = new Dictionary<string, object?>();
                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        keyValues[propertyName] = property.CurrentValue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            oldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                oldValues[propertyName] = property.OriginalValue;
                                newValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }

                auditEntry.KeyValues = JsonSerializer.Serialize(keyValues);
                auditEntry.OldValues = oldValues.Count > 0 ? JsonSerializer.Serialize(oldValues) : null;
                auditEntry.NewValues = newValues.Count > 0 ? JsonSerializer.Serialize(newValues) : null;

                auditEntries.Add(auditEntry);
            }

            if (auditEntries.Any())
            {
                await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

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
