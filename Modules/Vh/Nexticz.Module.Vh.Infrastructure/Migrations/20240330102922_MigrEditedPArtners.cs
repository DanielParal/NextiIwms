using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexticz.Module.Vh.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrEditedPArtners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PackageCoefficient",
                schema: "Vh",
                table: "Partners",
                newName: "ReceiptCoefficient");

            migrationBuilder.RenameColumn(
                name: "Package",
                schema: "Vh",
                table: "Partners",
                newName: "Receipt");

            migrationBuilder.RenameColumn(
                name: "IncomeCoefficient",
                schema: "Vh",
                table: "Partners",
                newName: "PackagingCoefficient");

            migrationBuilder.RenameColumn(
                name: "Income",
                schema: "Vh",
                table: "Partners",
                newName: "Packaging");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptCoefficient",
                schema: "Vh",
                table: "Partners",
                newName: "PackageCoefficient");

            migrationBuilder.RenameColumn(
                name: "Receipt",
                schema: "Vh",
                table: "Partners",
                newName: "Package");

            migrationBuilder.RenameColumn(
                name: "PackagingCoefficient",
                schema: "Vh",
                table: "Partners",
                newName: "IncomeCoefficient");

            migrationBuilder.RenameColumn(
                name: "Packaging",
                schema: "Vh",
                table: "Partners",
                newName: "Income");
        }
    }
}
