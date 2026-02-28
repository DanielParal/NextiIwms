using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_Added_LoadingActivitiesPickingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PickingPlaceHight",
                schema: "Vh",
                table: "LoadedActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickingPlaceHight",
                schema: "Vh",
                table: "LoadedActivities");
        }
    }
}
