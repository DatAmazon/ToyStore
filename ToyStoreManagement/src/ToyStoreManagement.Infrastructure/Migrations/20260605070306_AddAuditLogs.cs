using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_LOGS",
                schema: "C##ORACLEDB2026",
                columns: table => new
                {
                    AUDIT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TABLE_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ACTION = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    KEY_VALUES = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OLD_VALUES = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NEW_VALUES = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TIMESTAMP = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    USER_ID = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOGS", x => x.AUDIT_ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_LOGS",
                schema: "C##ORACLEDB2026");
        }
    }
}
