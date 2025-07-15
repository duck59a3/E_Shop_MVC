using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "Id");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 21, 50, 801, DateTimeKind.Local).AddTicks(6197), new DateTime(2025, 3, 11, 16, 21, 50, 801, DateTimeKind.Local).AddTicks(6213) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 21, 50, 801, DateTimeKind.Local).AddTicks(6219), new DateTime(2025, 3, 11, 16, 21, 50, 801, DateTimeKind.Local).AddTicks(6220) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Products",
                newName: "ProductId");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9947), new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9961) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9967), new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9968) });
        }
    }
}
