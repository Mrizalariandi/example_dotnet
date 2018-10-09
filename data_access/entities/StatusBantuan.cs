using System;
using System.ComponentModel.DataAnnotations;

namespace data_access.entities
{
    /// <summary>
    /// Status bantuan. 1. Sedang Diproses 2. Laporan Diterima 3. Laporan Dianalisa 4. Laporan Ditolak 5. Laporan  Selesai
    /// 
    /// </summary>
    public class StatusBantuan
    {
        [Key]
        public Int16 ID { set; get; }
        public string Name { set; get; }
    }
}
