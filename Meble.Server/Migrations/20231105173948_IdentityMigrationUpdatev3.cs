using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class IdentityMigrationUpdatev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_Users_UserDataId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "UserDatas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

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
                name: "UserId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.AddColumn<string>(
                name: "UserDataId",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatas_Users_UserDataId",
                schema: "dbo",
                table: "UserDatas",
                column: "UserDataId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
