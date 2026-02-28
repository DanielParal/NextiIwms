using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrEditShiftMasterChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "WorkerShiftChanges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerShiftActivityId",
                schema: "Vh",
                table: "WorkerShiftActivityChanges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkerShiftId",
                schema: "Vh",
                table: "WorkerShiftChanges");

            migrationBuilder.DropColumn(
                name: "WorkerShiftActivityId",
                schema: "Vh",
                table: "WorkerShiftActivityChanges");
        }
    }
}
