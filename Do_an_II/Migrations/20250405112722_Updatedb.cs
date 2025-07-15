using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class Updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 5, 18, 27, 19, 676, DateTimeKind.Local).AddTicks(7375), new DateTime(2025, 4, 5, 18, 27, 19, 676, DateTimeKind.Local).AddTicks(7394) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 5, 18, 27, 19, 676, DateTimeKind.Local).AddTicks(7404), new DateTime(2025, 4, 5, 18, 27, 19, 676, DateTimeKind.Local).AddTicks(7404) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 17, 9, 24, 76, DateTimeKind.Local).AddTicks(56), new DateTime(2025, 3, 11, 17, 9, 24, 76, DateTimeKind.Local).AddTicks(69) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 17, 9, 24, 76, DateTimeKind.Local).AddTicks(81), new DateTime(2025, 3, 11, 17, 9, 24, 76, DateTimeKind.Local).AddTicks(81) });
        }
    }
}
