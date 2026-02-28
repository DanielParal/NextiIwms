using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedSystemActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemActivities",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SystemType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepositorGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WhatToMeasure = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Coefficient = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    CutOff = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemActivities_Centers_CenterId",
                        column: x => x.CenterId,
                        principalSchema: "Vh",
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemActivities_DepositorsGroups_DepositorGroupId",
                        column: x => x.DepositorGroupId,
                        principalSchema: "Vh",
                        principalTable: "DepositorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemActivities_CenterId",
                schema: "Vh",
                table: "SystemActivities",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemActivities_DepositorGroupId",
                schema: "Vh",
                table: "SystemActivities",
                column: "DepositorGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemActivities",
                schema: "Vh");
        }
    }
}
