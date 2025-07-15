using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImgProductUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2104), new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2115) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2122), new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2122) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
