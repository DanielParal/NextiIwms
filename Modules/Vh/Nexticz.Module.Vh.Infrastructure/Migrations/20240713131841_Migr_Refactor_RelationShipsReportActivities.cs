using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migr_Refactor_RelationShipsReportActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReportActivities_WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities",
                column: "WorkerShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActivities_WorkerShifts_WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities",
                column: "WorkerShiftId",
                principalSchema: "Vh",
                principalTable: "WorkerShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActivities_WorkerShifts_WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities");

            migrationBuilder.DropIndex(
                name: "IX_ReportActivities_WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities");

            migrationBuilder.DropColumn(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "ReportActivities");
        }
    }
}
