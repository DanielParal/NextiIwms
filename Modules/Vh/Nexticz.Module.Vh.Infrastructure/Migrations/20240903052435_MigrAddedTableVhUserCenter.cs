using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedTableVhUserCenter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VhUserCenter",
                schema: "Vh",
                columns: table => new
                {
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VhUserCenter", x => new { x.CenterId, x.UserId });
                    table.ForeignKey(
                        name: "FK_VhUserCenter_Centers_CenterId",
                        column: x => x.CenterId,
                        principalSchema: "Vh",
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VhUserCenter_VhUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Vh",
                        principalTable: "VhUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VhUserCenter_UserId",
                schema: "Vh",
                table: "VhUserCenter",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VhUserCenter",
                schema: "Vh");
        }
    }
}
