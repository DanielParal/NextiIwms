using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedDepositors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Depositors",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositorsGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Depositors_Centers_CenterId",
                        column: x => x.CenterId,
                        principalSchema: "Vh",
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Depositors_DepositorsGroups_DepositorsGroupId",
                        column: x => x.DepositorsGroupId,
                        principalSchema: "Vh",
                        principalTable: "DepositorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depositors_CenterId",
                schema: "Vh",
                table: "Depositors",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Depositors_DepositorsGroupId",
                schema: "Vh",
                table: "Depositors",
                column: "DepositorsGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depositors",
                schema: "Vh");
        }
    }
}
