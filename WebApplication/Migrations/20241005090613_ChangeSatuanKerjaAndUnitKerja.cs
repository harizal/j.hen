using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSatuanKerjaAndUnitKerja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.AddColumn<string>(
                name: "SatuanKerjaID",
                table: "Poldas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SatuanKerjaID",
                table: "Poldas");

            migrationBuilder.AddColumn<string>(
                name: "UnitKerjaID",
                table: "SatuanKerjas",
                type: "TEXT",
                nullable: true);
        }
    }
}
