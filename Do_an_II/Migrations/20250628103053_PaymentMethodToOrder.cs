using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class PaymentMethodToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 28, 17, 30, 50, 344, DateTimeKind.Local).AddTicks(2662), new DateTime(2025, 6, 28, 17, 30, 50, 344, DateTimeKind.Local).AddTicks(2675) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 28, 17, 30, 50, 344, DateTimeKind.Local).AddTicks(2682), new DateTime(2025, 6, 28, 17, 30, 50, 344, DateTimeKind.Local).AddTicks(2682) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 6, 22, 54, 51, 524, DateTimeKind.Local).AddTicks(5442), new DateTime(2025, 6, 6, 22, 54, 51, 524, DateTimeKind.Local).AddTicks(5454) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 6, 22, 54, 51, 524, DateTimeKind.Local).AddTicks(5460), new DateTime(2025, 6, 6, 22, 54, 51, 524, DateTimeKind.Local).AddTicks(5460) });
        }
    }
}
