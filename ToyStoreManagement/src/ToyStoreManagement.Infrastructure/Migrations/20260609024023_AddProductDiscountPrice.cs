using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDiscountPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DISCOUNT_PRICE",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NUMBER(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DISCOUNT_PRICE",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");
        }
    }
}
