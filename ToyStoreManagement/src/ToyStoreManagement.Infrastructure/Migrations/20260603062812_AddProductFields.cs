using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProductFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BADGE",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BADGE_COLOR",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "IMG",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RATING",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "BINARY_DOUBLE",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "REVIEWS",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NUMBER(10)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BADGE",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "BADGE_COLOR",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "IMG",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "RATING",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "REVIEWS",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");
        }
    }
}
