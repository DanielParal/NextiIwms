using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_WorkershoftActtivity_Added_DetailedInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityDetails",
                schema: "Vh",
                table: "WorkerShiftActivities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityDetails",
                schema: "Vh",
                table: "WorkerShiftActivities");
        }
    }
}
