using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrRefactorWorkerShiftProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActivityCountOrDurationMinutes",
                schema: "Vh",
                table: "WorkerShiftActivity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Coefficient",
                schema: "Vh",
                table: "WorkerShiftActivity",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DurationMinutes",
                schema: "Vh",
                table: "WorkerShiftActivity",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                schema: "Vh",
                table: "WorkerShiftActivity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                schema: "Vh",
                table: "WorkerShiftActivity",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Coefficient",
                schema: "Vh",
                table: "LoadedActivities",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
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
                name: "ActivityCountOrDurationMinutes",
                schema: "Vh",
                table: "WorkerShiftActivity");

            migrationBuilder.DropColumn(
                name: "Coefficient",
                schema: "Vh",
                table: "WorkerShiftActivity");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                schema: "Vh",
                table: "WorkerShiftActivity");

            migrationBuilder.DropColumn(
                name: "Score",
                schema: "Vh",
                table: "WorkerShiftActivity");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "Vh",
                table: "WorkerShiftActivity");

            migrationBuilder.DropColumn(
                name: "Coefficient",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "Vh",
                table: "LoadedActivities");
        }
    }
}
