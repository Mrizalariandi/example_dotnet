using System;
using System.ComponentModel.DataAnnotations;

namespace contracts.Requests
{
    public class FormBantuanHukumRequest
    {
        public long ID { set; get; }
        [Required]
        public string NamaLengkap { set; get; }
        [Required]
        public int Umur { set; get; }
        [Required]
        public string Pekerjaan { set; get; }
        [Required]
        public string JenisKelamin { set; get; }
        [Required]
        public string Agama { set; get; }
        [Required]
        public int Kewarganegaraan { set; get; }
        [Required]
        public string Telpn { set; get; }
        [Required]
        public string Identitas { set; get; }
        [Required]
        public string NoIdentitas { set; get; }
        [Required]
        [EmailAddress]
        public string Email { set; get; }
        [Required]
        public string IsiPermohonan { set; get; }
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
    }
}
