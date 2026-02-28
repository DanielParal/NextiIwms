using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedActivityAfterCutOffCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityAfterCutOffCode",
                schema: "Vh",
                table: "WorkerShifts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActivityAfterCutOffCode",
                schema: "Vh",
                table: "Workers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityAfterCutOffCode",
                schema: "Vh",
                table: "WorkerShifts");

            migrationBuilder.DropColumn(
                name: "ActivityAfterCutOffCode",
                schema: "Vh",
                table: "Workers");
        }
    }
}
