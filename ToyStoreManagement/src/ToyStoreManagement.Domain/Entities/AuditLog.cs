using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyStoreManagement.Domain.Entities
{
    [Table("AUDIT_LOGS")]
    public class AuditLog
    {
        [Key]
        [Column("AUDIT_ID")]
        public Guid AuditId { get; set; } = Guid.NewGuid();

        [Column("TABLE_NAME")]
        [StringLength(100)]
        public string TableName { get; set; } = string.Empty;

        [Column("ACTION")]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete

        [Column("KEY_VALUES")]
        public string KeyValues { get; set; } = string.Empty;

        [Column("OLD_VALUES")]
        public string? OldValues { get; set; }

        [Column("NEW_VALUES")]
        public string? NewValues { get; set; }

        [Column("TIMESTAMP")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column("USER_ID")]
        [StringLength(255)]
        public string UserId { get; set; } = "System";
    }
}
