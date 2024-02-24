using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedClientOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalItemsOrdered",
                schema: "dbo",
                table: "ClientOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOrderValue",
                schema: "dbo",
                table: "ClientOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalItemsOrdered",
                schema: "dbo",
                table: "ClientOrders");

            migrationBuilder.DropColumn(
                name: "TotalOrderValue",
                schema: "dbo",
                table: "ClientOrders");
        }
    }
}
