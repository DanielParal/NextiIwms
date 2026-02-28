using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddUserDefaultUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultUrl",
                schema: "Auth",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultUrl",
                schema: "Auth",
                table: "AppUsers");
        }
    }
}
