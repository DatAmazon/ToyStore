using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class AllowGuestCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ORDERS_CUSTOMER_CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS");

            migrationBuilder.AlterColumn<Guid>(
                name: "CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS",
                type: "RAW(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "RAW(16)");

            migrationBuilder.AddForeignKey(
                name: "FK_ORDERS_CUSTOMER_CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS",
                column: "CUSTOMER_ID",
                principalSchema: "C##ORACLEDB2026",
                principalTable: "CUSTOMER",
                principalColumn: "CUSTOMER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ORDERS_CUSTOMER_CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS");

            migrationBuilder.AlterColumn<Guid>(
                name: "CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "RAW(16)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ORDERS_CUSTOMER_CUSTOMER_ID",
                schema: "C##ORACLEDB2026",
                table: "ORDERS",
                column: "CUSTOMER_ID",
                principalSchema: "C##ORACLEDB2026",
                principalTable: "CUSTOMER",
                principalColumn: "CUSTOMER_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
