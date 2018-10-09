using System;
namespace contracts.Responses
{
    public class StatusResponse
    {
        public int ID { set; get; }
        public string Name { set; get; }
    }
    public class ListPermohonanResponse
    {
        public long ID { set; get; }
        public string NamaLengkap { set; get; }
        public int Umur { set; get; }
        public string Pekerjaan { set; get; }
        public string JenisKelamin { set; get; }
        public string Agama { set; get; }
        public int Kewarganegaraan { set; get; }
        public string KewarganegaraanText { set; get; }
        public string Telpn { set; get; }
        public string Identitas { set; get; }
        public string NoIdentitas { set; get; }
        public string Email { set; get; }
        public string IsiPermohonan { set; get; }
        public string FileName { set; get; }
        public string KodeLaporan { set; get; }
        public DateTime Tanggal { set; get; }
        public bool IsLaporanDiterima { set; get; }
        public bool IsLaporanDitolak { set; get; }
        public bool IsLaporanDianalisa { set; get; }
        public bool IsLaporanSelesai { set; get; }
        public bool IsLaporanDiproses { set; get; }

    }
}
