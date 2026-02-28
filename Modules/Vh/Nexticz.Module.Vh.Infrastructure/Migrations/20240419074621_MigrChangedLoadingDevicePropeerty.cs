using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrChangedLoadingDevicePropeerty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodeWms",
                schema: "Vh",
                table: "LoadingDevices",
                newName: "LastActivityWorkerCodeWms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastActivityWorkerCodeWms",
                schema: "Vh",
                table: "LoadingDevices",
                newName: "CodeWms");
        }
    }
}
