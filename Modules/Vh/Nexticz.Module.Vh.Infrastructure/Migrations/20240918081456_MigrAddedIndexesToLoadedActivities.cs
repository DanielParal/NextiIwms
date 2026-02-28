using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedIndexesToLoadedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ActivitySource",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LoadedActivities_ActivitySource",
                schema: "Vh",
                table: "LoadedActivities",
                column: "ActivitySource");

            migrationBuilder.CreateIndex(
                name: "IX_LoadedActivities_ActivitySource_Created",
                schema: "Vh",
                table: "LoadedActivities",
                columns: new[] { "ActivitySource", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_LoadedActivities_Created",
                schema: "Vh",
                table: "LoadedActivities",
                column: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoadedActivities_ActivitySource",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropIndex(
                name: "IX_LoadedActivities_ActivitySource_Created",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropIndex(
                name: "IX_LoadedActivities_Created",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.AlterColumn<string>(
                name: "ActivitySource",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
