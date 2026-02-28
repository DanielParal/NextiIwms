using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrCjangedSystemActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemActivities_Centers_CenterId",
                schema: "Vh",
                table: "SystemActivities");

            migrationBuilder.RenameColumn(
                name: "CenterId",
                schema: "Vh",
                table: "SystemActivities",
                newName: "ActivityCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemActivities_CenterId",
                schema: "Vh",
                table: "SystemActivities",
                newName: "IX_SystemActivities_ActivityCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemActivities_ActivityCategories_ActivityCategoryId",
                schema: "Vh",
                table: "SystemActivities",
                column: "ActivityCategoryId",
                principalSchema: "Vh",
                principalTable: "ActivityCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemActivities_ActivityCategories_ActivityCategoryId",
                schema: "Vh",
                table: "SystemActivities");

            migrationBuilder.RenameColumn(
                name: "ActivityCategoryId",
                schema: "Vh",
                table: "SystemActivities",
                newName: "CenterId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemActivities_ActivityCategoryId",
                schema: "Vh",
                table: "SystemActivities",
                newName: "IX_SystemActivities_CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemActivities_Centers_CenterId",
                schema: "Vh",
                table: "SystemActivities",
                column: "CenterId",
                principalSchema: "Vh",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
