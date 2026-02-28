using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAppUserAddedBlockedFrom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserApiKeys_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRefreshTokens_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedFrom",
                schema: "Auth",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserApiKeys_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserApiKeys",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRefreshTokens_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRefreshTokens",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRoles",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserApiKeys_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRefreshTokens_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRoles");

            migrationBuilder.DropColumn(
                name: "BlockedFrom",
                schema: "Auth",
                table: "AppUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserApiKeys_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserApiKeys",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserClaims_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserClaims",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRefreshTokens_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRefreshTokens",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Auth",
                table: "AppUserRoles",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
