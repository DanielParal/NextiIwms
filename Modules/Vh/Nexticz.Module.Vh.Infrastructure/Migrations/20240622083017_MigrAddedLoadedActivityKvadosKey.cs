using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedLoadedActivityKvadosKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Idd",
                schema: "Vh",
                table: "LoadedActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Idi",
                schema: "Vh",
                table: "LoadedActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Idp",
                schema: "Vh",
                table: "LoadedActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Idt",
                schema: "Vh",
                table: "LoadedActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Idd",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "Idi",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "Idp",
                schema: "Vh",
                table: "LoadedActivities");

            migrationBuilder.DropColumn(
                name: "Idt",
                schema: "Vh",
                table: "LoadedActivities");
        }
    }
}
