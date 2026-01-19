using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderDetail2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Vouchers");

            migrationBuilder.AddColumn<string>(
                name: "FlashSaleId",
                table: "OrderDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlashSale",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 4, 12, 43, 51, 235, DateTimeKind.Local).AddTicks(7065), new DateTime(2026, 1, 4, 12, 43, 51, 235, DateTimeKind.Local).AddTicks(7079) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 4, 12, 43, 51, 235, DateTimeKind.Local).AddTicks(7086), new DateTime(2026, 1, 4, 12, 43, 51, 235, DateTimeKind.Local).AddTicks(7086) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlashSaleId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "IsFlashSale",
                table: "OrderDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 21, 9, 48, 10, 791, DateTimeKind.Local).AddTicks(5472), new DateTime(2025, 11, 21, 9, 48, 10, 791, DateTimeKind.Local).AddTicks(5490) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 21, 9, 48, 10, 791, DateTimeKind.Local).AddTicks(5504), new DateTime(2025, 11, 21, 9, 48, 10, 791, DateTimeKind.Local).AddTicks(5505) });
        }
    }
}
