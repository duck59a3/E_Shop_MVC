using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class AddProductReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 48, 1, 410, DateTimeKind.Local).AddTicks(9579), new DateTime(2025, 6, 1, 17, 48, 1, 410, DateTimeKind.Local).AddTicks(9592) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 48, 1, 410, DateTimeKind.Local).AddTicks(9621), new DateTime(2025, 6, 1, 17, 48, 1, 410, DateTimeKind.Local).AddTicks(9622) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 13, 52, 44, 615, DateTimeKind.Local).AddTicks(2507), new DateTime(2025, 5, 26, 13, 52, 44, 615, DateTimeKind.Local).AddTicks(2521) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 13, 52, 44, 615, DateTimeKind.Local).AddTicks(2529), new DateTime(2025, 5, 26, 13, 52, 44, 615, DateTimeKind.Local).AddTicks(2530) });
        }
    }
}
