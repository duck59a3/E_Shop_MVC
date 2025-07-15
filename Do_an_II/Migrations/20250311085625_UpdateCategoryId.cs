using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "Áo thời trang", 1, "Áo" },
                    { 2, "Quần thời trang", 2, "Quần dài" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Color", "CreatedAt", "Description", "ImageUrl", "IsActive", "Material", "Name", "Price", "Quantity", "Size", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Đen", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6027), "Áo thun nam", "~/img/product1", true, "Cotton", "Áo thun", 100m, 10, "M", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6038) },
                    { 2, 1, "Trắng", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6051), "Áo sơ mi nam", "~/img/product2", true, "Vải", "Áo sơ mi", 200m, 20, "L", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6051) }
                });
        }
    }
}
