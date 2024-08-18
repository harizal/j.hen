using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class AlterPegawaiAndAddSatuanKerja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KodeJabatanTerakhir",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "KodePendidikanTerakhir",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "KodeTempatLahir",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NamaJabatanTerakhir",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NamaPendidikanTerakhir",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NamaSekolah",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NomorIjazah",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NomorPeserta",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "NomorSK",
                table: "Pegawais");

            migrationBuilder.DropColumn(
                name: "PoldaID",
                table: "Pegawais");

            migrationBuilder.RenameColumn(
                name: "TanggalSK",
                table: "Pegawais",
                newName: "UnitKerjaID");

            migrationBuilder.RenameColumn(
                name: "TanggalLulus",
                table: "Pegawais",
                newName: "StatusPerkawinanID");

            migrationBuilder.RenameColumn(
                name: "StatusID",
                table: "Pegawais",
                newName: "SatuanKerjaID");

            migrationBuilder.CreateTable(
                name: "SatuanKerjas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatuanKerjas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SatuanKerjas");

            migrationBuilder.RenameColumn(
                name: "UnitKerjaID",
                table: "Pegawais",
                newName: "TanggalSK");

            migrationBuilder.RenameColumn(
                name: "StatusPerkawinanID",
                table: "Pegawais",
                newName: "TanggalLulus");

            migrationBuilder.RenameColumn(
                name: "SatuanKerjaID",
                table: "Pegawais",
                newName: "StatusID");

            migrationBuilder.AddColumn<string>(
                name: "KodeJabatanTerakhir",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KodePendidikanTerakhir",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KodeTempatLahir",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamaJabatanTerakhir",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamaPendidikanTerakhir",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamaSekolah",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomorIjazah",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomorPeserta",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomorSK",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoldaID",
                table: "Pegawais",
                type: "TEXT",
                nullable: true);
        }
    }
}
