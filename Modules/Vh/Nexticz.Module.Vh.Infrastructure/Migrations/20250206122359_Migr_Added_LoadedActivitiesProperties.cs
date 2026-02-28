using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_Added_LoadedActivitiesProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicenceKod",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PDoklad",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SortKod",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenceKod",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "PDoklad",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "SortKod",
                schema: "Vh",
                table: "LoadedActivities");
        }
    }
}
