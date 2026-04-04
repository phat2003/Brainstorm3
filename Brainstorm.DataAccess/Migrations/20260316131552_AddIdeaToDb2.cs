using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brainstorm.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIdeaToDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Ideas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_ApplicationUserId",
                table: "Ideas",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_AspNetUsers_ApplicationUserId",
                table: "Ideas",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_AspNetUsers_ApplicationUserId",
                table: "Ideas");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_ApplicationUserId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Ideas");
        }
    }
}
