using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductImageStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMG",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.AddColumn<string>(
                name: "IMAGE_URL",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMAGE_URL",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.AddColumn<byte[]>(
                name: "IMG",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "BLOB",
                nullable: true);
        }
    }
}
