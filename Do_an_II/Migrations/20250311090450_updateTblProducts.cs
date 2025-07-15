using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class updateTblProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Color", "CreatedAt", "Description", "ImageUrl", "IsActive", "Material", "Name", "Price", "Quantity", "Size", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 7, "Đen", new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9947), "Áo thun nam", "~/img/product1", true, "Cotton", "Áo thun", 100m, 10, "M", new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9961) },
                    { 2, 9, "Trắng", new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9967), "Áo sơ mi nam", "~/img/product2", true, "Vải", "Áo sơ mi", 200m, 20, "L", new DateTime(2025, 3, 11, 16, 4, 49, 558, DateTimeKind.Local).AddTicks(9968) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);
        }
    }
}
