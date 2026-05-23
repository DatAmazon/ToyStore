using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateToyStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORY",
                columns: table => new
                {
                    CATEGORY_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CATEGORY_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORY", x => x.CATEGORY_ID);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER",
                columns: table => new
                {
                    CUSTOMER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    PHONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    ADDRESS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.CUSTOMER_ID);
                });

            migrationBuilder.CreateTable(
                name: "SUPPLIERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUPPLIERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT",
                columns: table => new
                {
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    PRICE = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    STOCK_QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MINIMUM_AGE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MANUFACTURER = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CATEGORY_ID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT", x => x.PRODUCT_ID);
                    table.ForeignKey(
                        name: "FK_PRODUCT_CATEGORY_CATEGORY_ID",
                        column: x => x.CATEGORY_ID,
                        principalTable: "CATEGORY",
                        principalColumn: "CATEGORY_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ORDER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ORDER_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    DISCOUNT = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    FINAL_AMOUNT = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    CUSTOMER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CUSTOMER_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CUSTOMER_PHONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    SHIPPING_ADDRESS = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.ORDER_ID);
                    table.ForeignKey(
                        name: "FK_ORDERS_CUSTOMER_CUSTOMER_ID",
                        column: x => x.CUSTOMER_ID,
                        principalTable: "CUSTOMER",
                        principalColumn: "CUSTOMER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INVENTORY_RECEIPTS",
                columns: table => new
                {
                    INVENTORY_RECEIPT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RECEIVED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    SUPPLIER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTORY_RECEIPTS", x => x.INVENTORY_RECEIPT_ID);
                    table.ForeignKey(
                        name: "FK_INVENTORY_RECEIPTS_SUPPLIERS_SUPPLIER_ID",
                        column: x => x.SUPPLIER_ID,
                        principalTable: "SUPPLIERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CART_ITEM",
                columns: table => new
                {
                    CART_ITEM_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CUSTOMER_ID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CART_ITEM", x => x.CART_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_CART_ITEM_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_DETAILS",
                columns: table => new
                {
                    ORDER_DETAIL_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ORDER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PRICE = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_DETAILS", x => x.ORDER_DETAIL_ID);
                    table.ForeignKey(
                        name: "FK_ORDER_DETAILS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ORDER_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_DETAILS_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INVENTORY_RECEIPT_DETAILS",
                columns: table => new
                {
                    INVENTORY_RECEIPT_DETAIL_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    INVENTORY_RECEIPT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTORY_RECEIPT_DETAILS", x => x.INVENTORY_RECEIPT_DETAIL_ID);
                    table.ForeignKey(
                        name: "FK_INVENTORY_RECEIPT_DETAILS_INVENTORY_RECEIPTS_INVENTORY_RECEIPT_ID",
                        column: x => x.INVENTORY_RECEIPT_ID,
                        principalTable: "INVENTORY_RECEIPTS",
                        principalColumn: "INVENTORY_RECEIPT_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_INVENTORY_RECEIPT_DETAILS_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEM_PRODUCT_ID",
                table: "CART_ITEM",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_INVENTORY_RECEIPT_DETAILS_INVENTORY_RECEIPT_ID",
                table: "INVENTORY_RECEIPT_DETAILS",
                column: "INVENTORY_RECEIPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_INVENTORY_RECEIPT_DETAILS_PRODUCT_ID",
                table: "INVENTORY_RECEIPT_DETAILS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_INVENTORY_RECEIPTS_SUPPLIER_ID",
                table: "INVENTORY_RECEIPTS",
                column: "SUPPLIER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DETAILS_ORDER_ID",
                table: "ORDER_DETAILS",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DETAILS_PRODUCT_ID",
                table: "ORDER_DETAILS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_CUSTOMER_ID",
                table: "ORDERS",
                column: "CUSTOMER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_CATEGORY_ID",
                table: "PRODUCT",
                column: "CATEGORY_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CART_ITEM");

            migrationBuilder.DropTable(
                name: "INVENTORY_RECEIPT_DETAILS");

            migrationBuilder.DropTable(
                name: "ORDER_DETAILS");

            migrationBuilder.DropTable(
                name: "INVENTORY_RECEIPTS");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "PRODUCT");

            migrationBuilder.DropTable(
                name: "SUPPLIERS");

            migrationBuilder.DropTable(
                name: "CUSTOMER");

            migrationBuilder.DropTable(
                name: "CATEGORY");
        }
    }
}
