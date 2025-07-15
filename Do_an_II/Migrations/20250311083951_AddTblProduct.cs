using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class AddTblProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Color", "CreatedAt", "Description", "ImageUrl", "IsActive", "Material", "Name", "Price", "Quantity", "Size", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 7, "Đen", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6027), "Áo thun nam", "~/img/product1", true, "Cotton", "Áo thun", 100m, 10, "M", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6038) },
                    { 2, 9, "Trắng", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6051), "Áo sơ mi nam", "~/img/product2", true, "Vải", "Áo sơ mi", 200m, 20, "L", new DateTime(2025, 3, 11, 15, 39, 49, 279, DateTimeKind.Local).AddTicks(6051) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
