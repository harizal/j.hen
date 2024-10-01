using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class FixRellationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SatuanKerjas_Poldas_ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_SatuanKerjas_ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.DropColumn(
                name: "ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.CreateIndex(
                name: "IX_SatuanKerjas_UnitKerjaID",
                table: "SatuanKerjas",
                column: "UnitKerjaID");

            migrationBuilder.AddForeignKey(
                name: "FK_SatuanKerjas_Poldas_UnitKerjaID",
                table: "SatuanKerjas",
                column: "UnitKerjaID",
                principalTable: "Poldas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SatuanKerjas_Poldas_UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_SatuanKerjas_UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.AddColumn<string>(
                name: "ProdiID",
                table: "SatuanKerjas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SatuanKerjas_ProdiID",
                table: "SatuanKerjas",
                column: "ProdiID");

            migrationBuilder.AddForeignKey(
                name: "FK_SatuanKerjas_Poldas_ProdiID",
                table: "SatuanKerjas",
                column: "ProdiID",
                principalTable: "Poldas",
                principalColumn: "Id");
        }
    }
}
