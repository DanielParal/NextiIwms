using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrAddedShiftMasterChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftMasterChanges",
                schema: "Vh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftMasterActivityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContextChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CenterCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Values = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftMasterChanges", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftMasterChanges",
                schema: "Vh");
        }
    }
}
