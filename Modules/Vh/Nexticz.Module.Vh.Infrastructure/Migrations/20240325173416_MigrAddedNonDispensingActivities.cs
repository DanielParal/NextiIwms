using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedNonDispensingActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NonDispensingActivities",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RequireNote = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Coefficient = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CutOff = table.Column<TimeOnly>(type: "time", nullable: true),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonDispensingActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonDispensingActivities_ActivityCategories_ActivityCategoryId",
                        column: x => x.ActivityCategoryId,
                        principalSchema: "Vh",
                        principalTable: "ActivityCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonDispensingActivities_Centers_CenterId",
                        column: x => x.CenterId,
                        principalSchema: "Vh",
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NonDispensingActivities_ActivityCategoryId",
                schema: "Vh",
                table: "NonDispensingActivities",
                column: "ActivityCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NonDispensingActivities_CenterId",
                schema: "Vh",
                table: "NonDispensingActivities",
                column: "CenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonDispensingActivities",
                schema: "Vh");
        }
    }
}
