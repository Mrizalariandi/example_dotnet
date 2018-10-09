using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data_access.entities
{
    public class BantuanHukum
    {
        [Key]
        public long ID { set; get; }
        public string KodeLaporan { set; get; }
        public string NamaLengkap { set; get; }
        public int Umur { set; get; }
        public string Pekerjaan { set; get; }
        public string JenisKelamin { set; get; }
        public string Agama { set; get; }
        public Country Kewarganegaraan { set; get; }
        public string Telpn { set; get; }
        public string Identitas { set; get; }
        public string NoIdentitas { set; get; }
        public string Email { set; get; }
        public string IsiPermohonan { set; get; }
        public string FileName { set; get; }
        [ForeignKey("StatusBantuanForeignKey")]
        public StatusBantuan LastStatus { set; get; }
       
        public DateTime CreatedDate { set; get; }

    }
}
