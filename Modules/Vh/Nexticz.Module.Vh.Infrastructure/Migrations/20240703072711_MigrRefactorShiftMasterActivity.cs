using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrRefactorShiftMasterActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerShiftActivity",
                schema: "Vh");

            migrationBuilder.DropColumn(
                name: "ContextChangeType",
                schema: "Vh",
                table: "ShiftMasterChanges");

            migrationBuilder.DropColumn(
                name: "Values",
                schema: "Vh",
                table: "ShiftMasterChanges");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "ShiftMasterChanges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "WorkerShiftActivities",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCutOff = table.Column<TimeOnly>(type: "time", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivitySource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActivitiesCount = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Coefficient = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActivityCountOrDurationMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    LastAddedActivityStart = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerShiftActivities", x => new { x.WorkerShiftId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkerShiftActivities_WorkerShifts_WorkerShiftId",
                        column: x => x.WorkerShiftId,
                        principalSchema: "Vh",
                        principalTable: "WorkerShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerShiftActivityChanges",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftMasterChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContextChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkerShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCutOff = table.Column<TimeOnly>(type: "time", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivitySource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActivitiesCount = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Coefficient = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActivityCountOrDurationMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    LastAddedActivityStart = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerShiftActivityChanges", x => new { x.ShiftMasterChangeId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkerShiftActivityChanges_ShiftMasterChanges_ShiftMasterChangeId",
                        column: x => x.ShiftMasterChangeId,
                        principalSchema: "Vh",
                        principalTable: "ShiftMasterChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerShiftChanges",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftMasterChangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContextChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkerCenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerShiftChanges", x => new { x.ShiftMasterChangeId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkerShiftChanges_ShiftMasterChanges_ShiftMasterChangeId",
                        column: x => x.ShiftMasterChangeId,
                        principalSchema: "Vh",
                        principalTable: "ShiftMasterChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerShiftActivities",
                schema: "Vh");

            migrationBuilder.DropTable(
                name: "WorkerShiftActivityChanges",
                schema: "Vh");

            migrationBuilder.DropTable(
                name: "WorkerShiftChanges",
                schema: "Vh");

            migrationBuilder.DropColumn(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "ShiftMasterChanges");

            migrationBuilder.AddColumn<string>(
                name: "ContextChangeType",
                schema: "Vh",
                table: "ShiftMasterChanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Values",
                schema: "Vh",
                table: "ShiftMasterChanges",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "WorkerShiftActivity",
                schema: "Vh",
                columns: table => new
                {
                    WorkerShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivitiesCount = table.Column<int>(type: "int", nullable: false),
                    ActivityCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCountOrDurationMinutes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActivityCutOff = table.Column<TimeOnly>(type: "time", nullable: true),
                    ActivitySource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Coefficient = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAddedActivityStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerShiftActivity", x => new { x.WorkerShiftId, x.Id });
                    table.ForeignKey(
                        name: "FK_WorkerShiftActivity_WorkerShifts_WorkerShiftId",
                        column: x => x.WorkerShiftId,
                        principalSchema: "Vh",
                        principalTable: "WorkerShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
