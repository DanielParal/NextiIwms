using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedLoadingActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Workers_CodeWms",
                schema: "Vh",
                table: "Workers",
                column: "CodeWms");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_NonDispensingActivities_ActivityIdentifier",
                schema: "Vh",
                table: "NonDispensingActivities",
                column: "ActivityIdentifier");

            migrationBuilder.CreateTable(
                name: "LoadingActionsNdas",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerSlug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NonDispensingActivitySlug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadingActionsNdas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadingActionsNdas_LoadingDevices_LoadingDeviceId",
                        column: x => x.LoadingDeviceId,
                        principalSchema: "Vh",
                        principalTable: "LoadingDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoadingActionsNdas_NonDispensingActivities_NonDispensingActivitySlug",
                        column: x => x.NonDispensingActivitySlug,
                        principalSchema: "Vh",
                        principalTable: "NonDispensingActivities",
                        principalColumn: "ActivityIdentifier",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoadingActionsNdas_Workers_WorkerSlug",
                        column: x => x.WorkerSlug,
                        principalSchema: "Vh",
                        principalTable: "Workers",
                        principalColumn: "CodeWms",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadingActionsNdas_LoadingDeviceId",
                schema: "Vh",
                table: "LoadingActionsNdas",
                column: "LoadingDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadingActionsNdas_NonDispensingActivitySlug",
                schema: "Vh",
                table: "LoadingActionsNdas",
                column: "NonDispensingActivitySlug");

            migrationBuilder.CreateIndex(
                name: "IX_LoadingActionsNdas_WorkerSlug",
                schema: "Vh",
                table: "LoadingActionsNdas",
                column: "WorkerSlug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadingActionsNdas",
                schema: "Vh");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Workers_CodeWms",
                schema: "Vh",
                table: "Workers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_NonDispensingActivities_ActivityIdentifier",
                schema: "Vh",
                table: "NonDispensingActivities");
        }
    }
}
