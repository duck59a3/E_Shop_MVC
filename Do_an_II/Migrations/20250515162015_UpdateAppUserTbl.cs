using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUserTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 15, 23, 20, 15, 26, DateTimeKind.Local).AddTicks(9318), new DateTime(2025, 5, 15, 23, 20, 15, 26, DateTimeKind.Local).AddTicks(9330) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 15, 23, 20, 15, 26, DateTimeKind.Local).AddTicks(9337), new DateTime(2025, 5, 15, 23, 20, 15, 26, DateTimeKind.Local).AddTicks(9337) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 15, 22, 44, 13, 789, DateTimeKind.Local).AddTicks(1418), new DateTime(2025, 5, 15, 22, 44, 13, 789, DateTimeKind.Local).AddTicks(1435) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 15, 22, 44, 13, 789, DateTimeKind.Local).AddTicks(1443), new DateTime(2025, 5, 15, 22, 44, 13, 789, DateTimeKind.Local).AddTicks(1443) });
        }
    }
}
