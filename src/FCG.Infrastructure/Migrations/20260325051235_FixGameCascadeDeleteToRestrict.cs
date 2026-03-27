using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixGameCascadeDeleteToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGameLibrary_Games_GameId",
                table: "UserGameLibrary");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGameLibrary_Games_GameId",
                table: "UserGameLibrary",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGameLibrary_Games_GameId",
                table: "UserGameLibrary");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGameLibrary_Games_GameId",
                table: "UserGameLibrary",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
