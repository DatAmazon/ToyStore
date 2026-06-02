using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "SUPPLIERS",
                newName: "SUPPLIERS",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "PRODUCT",
                newName: "PRODUCT",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "ORDERS",
                newName: "ORDERS",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "ORDER_DETAILS",
                newName: "ORDER_DETAILS",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "INVENTORY_RECEIPTS",
                newName: "INVENTORY_RECEIPTS",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "INVENTORY_RECEIPT_DETAILS",
                newName: "INVENTORY_RECEIPT_DETAILS",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "CUSTOMER",
                newName: "CUSTOMER",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "CATEGORY",
                newName: "CATEGORY",
                newSchema: "C##ORACLEDB2026");

            migrationBuilder.RenameTable(
                name: "CART_ITEM",
                newName: "CART_ITEM",
                newSchema: "C##ORACLEDB2026");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SUPPLIERS",
                schema: "C##ORACLEDB2026",
                newName: "SUPPLIERS");

            migrationBuilder.RenameTable(
                name: "PRODUCT",
                schema: "C##ORACLEDB2026",
                newName: "PRODUCT");

            migrationBuilder.RenameTable(
                name: "ORDERS",
                schema: "C##ORACLEDB2026",
                newName: "ORDERS");

            migrationBuilder.RenameTable(
                name: "ORDER_DETAILS",
                schema: "C##ORACLEDB2026",
                newName: "ORDER_DETAILS");

            migrationBuilder.RenameTable(
                name: "INVENTORY_RECEIPTS",
                schema: "C##ORACLEDB2026",
                newName: "INVENTORY_RECEIPTS");

            migrationBuilder.RenameTable(
                name: "INVENTORY_RECEIPT_DETAILS",
                schema: "C##ORACLEDB2026",
                newName: "INVENTORY_RECEIPT_DETAILS");

            migrationBuilder.RenameTable(
                name: "CUSTOMER",
                schema: "C##ORACLEDB2026",
                newName: "CUSTOMER");

            migrationBuilder.RenameTable(
                name: "CATEGORY",
                schema: "C##ORACLEDB2026",
                newName: "CATEGORY");

            migrationBuilder.RenameTable(
                name: "CART_ITEM",
                schema: "C##ORACLEDB2026",
                newName: "CART_ITEM");
        }
    }
}
