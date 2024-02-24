using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class LastMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfOrder",
                schema: "dbo",
                table: "ClientOrders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                schema: "dbo",
                table: "ClientOrders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                schema: "dbo",
                table: "ClientOrders");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfOrder",
                schema: "dbo",
                table: "ClientOrders",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
