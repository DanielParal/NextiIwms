using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_RemovedApiKeyClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserApiKeyClaims",
                schema: "Auth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserApiKeyClaims",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserApiKeyClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserApiKeyClaims_AppUserApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "Auth",
                        principalTable: "AppUserApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserApiKeyClaims_ApiKeyId",
                schema: "Auth",
                table: "AppUserApiKeyClaims",
                column: "ApiKeyId");
        }
    }
}
