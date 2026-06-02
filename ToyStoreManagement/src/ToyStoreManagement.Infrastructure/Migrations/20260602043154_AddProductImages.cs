using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUCTIMAGES",
                schema: "C##ORACLEDB2026",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProductId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsMain = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTIMAGES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCTIMAGES_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "C##ORACLEDB2026",
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTIMAGES_ProductId",
                schema: "C##ORACLEDB2026",
                table: "PRODUCTIMAGES",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUCTIMAGES",
                schema: "C##ORACLEDB2026");
        }
    }
}
