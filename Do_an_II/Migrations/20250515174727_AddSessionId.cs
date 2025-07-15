using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 0, 47, 26, 769, DateTimeKind.Local).AddTicks(3644), new DateTime(2025, 5, 16, 0, 47, 26, 769, DateTimeKind.Local).AddTicks(3659) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 0, 47, 26, 769, DateTimeKind.Local).AddTicks(3670), new DateTime(2025, 5, 16, 0, 47, 26, 769, DateTimeKind.Local).AddTicks(3670) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");

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
    }
}
