using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class removeReviewFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 4, 18, 19, 45, 765, DateTimeKind.Local).AddTicks(3240), new DateTime(2025, 6, 4, 18, 19, 45, 765, DateTimeKind.Local).AddTicks(3269) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 4, 18, 19, 45, 765, DateTimeKind.Local).AddTicks(3281), new DateTime(2025, 6, 4, 18, 19, 45, 765, DateTimeKind.Local).AddTicks(3281) });
        }
    }
}
