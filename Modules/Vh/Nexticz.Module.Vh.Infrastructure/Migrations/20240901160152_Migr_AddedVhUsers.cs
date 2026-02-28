using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_AddedVhUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VhUsers",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VhUsers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VhUsers",
                schema: "Vh");
        }
    }
}
