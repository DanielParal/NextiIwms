using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrReportAddedWorkerCenterCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkerCenterCode",
                schema: "Vh",
                table: "ReportActivities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReportPerformanceEvaluations",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WorkerShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerShiftStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkerShiftEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationTime = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    ScoreMyStock = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    ScoreIwms = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    ScoreSag = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    ScoreNonProductive = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Zone = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPerformanceEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPerformanceEvaluations_WorkerShifts_WorkerShiftId",
                        column: x => x.WorkerShiftId,
                        principalSchema: "Vh",
                        principalTable: "WorkerShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportPerformanceEvaluations_WorkerShiftId",
                schema: "Vh",
                table: "ReportPerformanceEvaluations",
                column: "WorkerShiftId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportPerformanceEvaluations",
                schema: "Vh");

            migrationBuilder.DropColumn(
                name: "WorkerCenterCode",
                schema: "Vh",
                table: "ReportActivities");
        }
    }
}
