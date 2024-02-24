using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_Users_UserId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.DropIndex(
                name: "IX_UserDatas_UserId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "UserDatas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                schema: "dbo",
                table: "Furnitures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "Furnitures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_UserId",
                schema: "dbo",
                table: "UserDatas",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_Users_UserId",
                schema: "dbo",
                table: "UserDatas",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_Users_UserId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.DropIndex(
                name: "IX_UserDatas_UserId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                schema: "dbo",
                table: "Furnitures");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "dbo",
                table: "Furnitures");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "UserDatas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_UserId",
                schema: "dbo",
                table: "UserDatas",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_Users_UserId",
                schema: "dbo",
                table: "UserDatas",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
