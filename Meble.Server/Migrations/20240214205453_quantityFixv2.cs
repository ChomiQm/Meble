using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class quantityFixv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "dbo",
                table: "OrderFurnitures",
                newName: "QuantityOrdered");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityOrdered",
                schema: "dbo",
                table: "OrderFurnitures",
                newName: "Quantity");
        }
    }
}
