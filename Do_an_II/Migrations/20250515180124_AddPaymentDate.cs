using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Do_an_II.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4674), new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4686) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4694), new DateTime(2025, 5, 16, 1, 1, 23, 647, DateTimeKind.Local).AddTicks(4694) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Orders");

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
    }
}
