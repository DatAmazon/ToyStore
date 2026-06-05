using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SEARCH_MANUFACTURER",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEARCH_NAME",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SEARCH_MANUFACTURER",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "SEARCH_NAME",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");
        }
    }
}
