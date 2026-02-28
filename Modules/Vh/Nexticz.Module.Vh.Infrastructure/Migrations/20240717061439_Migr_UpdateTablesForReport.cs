using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_UpdateTablesForReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivitySystemType",
                schema: "Vh",
                table: "WorkerShiftActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepositorCode",
                schema: "Vh",
                table: "WorkerShiftActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositorGroupCode",
                schema: "Vh",
                table: "WorkerShiftActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivitySystemType",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepositorCode",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositorGroupCode",
                schema: "Vh",
                table: "LoadedActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivitySystemType",
                schema: "Vh",
                table: "WorkerShiftActivities");

            migrationBuilder.DropColumn(
                name: "DepositorCode",
                schema: "Vh",
                table: "WorkerShiftActivities");

            migrationBuilder.DropColumn(
                name: "DepositorGroupCode",
                schema: "Vh",
                table: "WorkerShiftActivities");

            migrationBuilder.DropColumn(
                name: "ActivitySystemType",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "DepositorCode",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "DepositorGroupCode",
                schema: "Vh",
                table: "LoadedActivities");
        }
    }
}
