using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRellationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProdiID",
                table: "SatuanKerjas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitKerjaID",
                table: "SatuanKerjas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendidikanID",
                table: "Prodis",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SatuanKerjas_ProdiID",
                table: "SatuanKerjas",
                column: "ProdiID");

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
                name: "FK_SatuanKerjas_Poldas_ProdiID",
                table: "SatuanKerjas",
                column: "ProdiID",
                principalTable: "Poldas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prodis_Pendidikans_PendidikanID",
                table: "Prodis");

            migrationBuilder.DropForeignKey(
                name: "FK_SatuanKerjas_Poldas_ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_SatuanKerjas_ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.DropIndex(
                name: "IX_Prodis_PendidikanID",
                table: "Prodis");

            migrationBuilder.DropColumn(
                name: "ProdiID",
                table: "SatuanKerjas");

            migrationBuilder.DropColumn(
                name: "UnitKerjaID",
                table: "SatuanKerjas");

            migrationBuilder.DropColumn(
                name: "PendidikanID",
                table: "Prodis");
        }
    }
}
