using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUCTREVIEWS",
                schema: "C##ORACLEDB2026",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProductId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Rating = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Comment = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTREVIEWS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCTREVIEWS_ASPNETUSERS_UserId",
                        column: x => x.UserId,
                        principalSchema: "C##ORACLEDB2026",
                        principalTable: "ASPNETUSERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PRODUCTREVIEWS_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "C##ORACLEDB2026",
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTREVIEWS_ProductId",
                schema: "C##ORACLEDB2026",
                table: "PRODUCTREVIEWS",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTREVIEWS_UserId",
                schema: "C##ORACLEDB2026",
                table: "PRODUCTREVIEWS",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUCTREVIEWS",
                schema: "C##ORACLEDB2026");
        }
    }
}
