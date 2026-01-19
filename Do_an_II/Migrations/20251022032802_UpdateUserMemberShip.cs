using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserMemberShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalOrders",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalSpent",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOrders",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalSpent",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 18, 54, 39, 611, DateTimeKind.Local).AddTicks(5367), new DateTime(2025, 7, 27, 18, 54, 39, 611, DateTimeKind.Local).AddTicks(5385) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 27, 18, 54, 39, 611, DateTimeKind.Local).AddTicks(5392), new DateTime(2025, 7, 27, 18, 54, 39, 611, DateTimeKind.Local).AddTicks(5392) });
        }
    }
}
