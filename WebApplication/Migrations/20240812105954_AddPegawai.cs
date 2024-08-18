using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPegawai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pegawais",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    NIK = table.Column<string>(type: "TEXT", nullable: true),
                    Nomor = table.Column<string>(type: "TEXT", nullable: true),
                    NomorPeserta = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    KodeTempatLahir = table.Column<string>(type: "TEXT", nullable: true),
                    NamaTempatLahir = table.Column<string>(type: "TEXT", nullable: true),
                    TanggalLahir = table.Column<DateTime>(type: "TEXT", nullable: true),
                    JenisKelamin = table.Column<int>(type: "INTEGER", nullable: true),
                    KodePendidikanTerakhir = table.Column<string>(type: "TEXT", nullable: true),
                    NamaPendidikanTerakhir = table.Column<string>(type: "TEXT", nullable: true),
                    NomorIjazah = table.Column<string>(type: "TEXT", nullable: true),
                    NamaSekolah = table.Column<string>(type: "TEXT", nullable: true),
                    TanggalLulus = table.Column<DateTime>(type: "TEXT", nullable: true),
                    KodeJabatanTerakhir = table.Column<string>(type: "TEXT", nullable: true),
                    NamaJabatanTerakhir = table.Column<string>(type: "TEXT", nullable: true),
                    NomorSK = table.Column<string>(type: "TEXT", nullable: true),
                    TanggalSK = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TanggalAwal = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TanggalAkhir = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PoldaID = table.Column<string>(type: "TEXT", nullable: true),
                    PendidikanID = table.Column<string>(type: "TEXT", nullable: true),
                    ProdiID = table.Column<string>(type: "TEXT", nullable: true),
                    StatusID = table.Column<string>(type: "TEXT", nullable: true),
                    Keterangan = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pegawais", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pegawais");
        }
    }
}
