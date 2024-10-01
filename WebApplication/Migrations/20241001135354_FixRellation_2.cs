using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class FixRellation_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prodis_Pendidikans_PendidikanID",
                table: "Prodis");

            migrationBuilder.DropForeignKey(
                name: "FK_SatuanKerjas_Poldas_UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_SatuanKerjas_UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_Prodis_PendidikanID",
                table: "Prodis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SatuanKerjas_UnitKerjaID",
                table: "SatuanKerjas",
                column: "UnitKerjaID");

            migrationBuilder.CreateIndex(
                name: "IX_Prodis_PendidikanID",
                table: "Prodis",
                column: "PendidikanID");

            migrationBuilder.AddForeignKey(
                name: "FK_Prodis_Pendidikans_PendidikanID",
                table: "Prodis",
                column: "PendidikanID",
                principalTable: "Pendidikans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SatuanKerjas_Poldas_UnitKerjaID",
                table: "SatuanKerjas",
                column: "UnitKerjaID",
                principalTable: "Poldas",
                principalColumn: "Id");
        }
    }
}
