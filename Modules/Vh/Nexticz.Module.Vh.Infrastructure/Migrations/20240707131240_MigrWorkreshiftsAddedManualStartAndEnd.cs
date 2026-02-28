using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrWorkreshiftsAddedManualStartAndEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ManualEnd",
                schema: "Vh",
                table: "WorkerShifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ManualStart",
                schema: "Vh",
                table: "WorkerShifts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManualEnd",
                schema: "Vh",
                table: "WorkerShifts");

            migrationBuilder.DropColumn(
                name: "ManualStart",
                schema: "Vh",
                table: "WorkerShifts");
        }
    }
}
