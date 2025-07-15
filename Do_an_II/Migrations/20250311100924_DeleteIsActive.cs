using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Products");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2104), true, new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2115) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsActive", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2122), true, new DateTime(2025, 3, 11, 16, 55, 36, 464, DateTimeKind.Local).AddTicks(2122) });
        }
    }
}
