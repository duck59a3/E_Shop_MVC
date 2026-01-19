using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 5, 15, 17, 51, 941, DateTimeKind.Local).AddTicks(4434), new DateTime(2025, 11, 5, 15, 17, 51, 941, DateTimeKind.Local).AddTicks(4448) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 5, 15, 17, 51, 941, DateTimeKind.Local).AddTicks(4456), new DateTime(2025, 11, 5, 15, 17, 51, 941, DateTimeKind.Local).AddTicks(4457) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 22, 10, 27, 58, 949, DateTimeKind.Local).AddTicks(563), new DateTime(2025, 10, 22, 10, 27, 58, 949, DateTimeKind.Local).AddTicks(576) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 22, 10, 27, 58, 949, DateTimeKind.Local).AddTicks(584), new DateTime(2025, 10, 22, 10, 27, 58, 949, DateTimeKind.Local).AddTicks(585) });
        }
    }
}
