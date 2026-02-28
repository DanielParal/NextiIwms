using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedAppUserClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
