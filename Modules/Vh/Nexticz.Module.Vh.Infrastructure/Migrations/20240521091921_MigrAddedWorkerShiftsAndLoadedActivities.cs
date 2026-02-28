using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedWorkerShiftsAndLoadedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoadedActivities",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActivityCutOff = table.Column<TimeOnly>(type: "time", nullable: true),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    ActivitySource = table.Column<int>(type: "int", nullable: false),
                    ActivityState = table.Column<int>(type: "int", nullable: false),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActivitiesCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadedActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkerShifts",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkerShiftActivity",
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
                    ActivityType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ActivitySource = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    LoadingDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActivitiesCount = table.Column<int>(type: "int", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadedActivities",
                schema: "Vh");

            migrationBuilder.DropTable(
                name: "WorkerShiftActivity",
                schema: "Vh");

            migrationBuilder.DropTable(
                name: "WorkerShifts",
                schema: "Vh");
        }
    }
}
