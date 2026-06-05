using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddECommerceFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DESCRIPTION",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IS_DELETED",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WISHLIST",
                schema: "C##ORACLEDB2026",
                columns: table => new
                {
                    WISHLIST_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PRODUCT_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ADDED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WISHLIST", x => x.WISHLIST_ID);
                    table.ForeignKey(
                        name: "FK_WISHLIST_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalSchema: "C##ORACLEDB2026",
                        principalTable: "PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WISHLIST_PRODUCT_ID",
                schema: "C##ORACLEDB2026",
                table: "WISHLIST",
                column: "PRODUCT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WISHLIST",
                schema: "C##ORACLEDB2026");

            migrationBuilder.DropColumn(
                name: "DESCRIPTION",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");

            migrationBuilder.DropColumn(
                name: "IS_DELETED",
                schema: "C##ORACLEDB2026",
                table: "PRODUCT");
        }
    }
}
