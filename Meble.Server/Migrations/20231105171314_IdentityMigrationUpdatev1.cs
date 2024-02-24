using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meble.Server.Migrations
{
    /// <inheritdoc />
    public partial class IdentityMigrationUpdatev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_User",
                schema: "dbo",
                table: "UserDatas");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatas_Users_UserDataId",
                schema: "dbo",
                table: "UserDatas");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_User",
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
