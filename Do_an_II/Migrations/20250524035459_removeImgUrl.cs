using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class removeImgUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 24, 10, 54, 56, 40, DateTimeKind.Local).AddTicks(8765), new DateTime(2025, 5, 24, 10, 54, 56, 40, DateTimeKind.Local).AddTicks(8777) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 24, 10, 54, 56, 40, DateTimeKind.Local).AddTicks(8785), new DateTime(2025, 5, 24, 10, 54, 56, 40, DateTimeKind.Local).AddTicks(8785) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4674), "~/img/product1", new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4686) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4694), "~/img/product2", new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4694) });
        }
    }
}
