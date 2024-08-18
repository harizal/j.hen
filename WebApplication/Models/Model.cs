using static WApp.Utlis.Enums;

namespace WApp.Models
{
    public class PoldaModel : BaseModel
    {
        public string Name { get; set; }
    }

    public class SatuanKerjaWilayahModel : BaseModel
    {
        public string Name { get; set; }
    }

    public class PendidikanModel : BaseModel
    {
        public string Name { get; set; }
    }

    public class ProdiModel : BaseModel
    {
        public string Name { get; set; }
    }

    public class StatusModel : BaseModel
    {
        public string Name { get; set; }
    }

    public class PegawaiModel : BaseModel
    {
        public PegawaiType Type { get; set; }
        public string? Nomor { get; set; }
        public string? NIK { get; set; }
        public string? Name { get; set; }

        public JenisKelamin? JenisKelamin { get; set; }        
        public string? NamaTempatLahir { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string? StatusPerkawinanID { get; set; }

        public string? PendidikanID { get; set; }
        public string? ProdiID { get; set; }
        public string? SatuanKerjaID { get; set; }
        public string? UnitKerjaID { get; set; }


        public DateTime? TanggalAwal { get; set; }
        public DateTime? TanggalAkhir { get; set; }

        public string? Keterangan { get; set; }

    }
}
