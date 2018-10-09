using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data_access.entities
{
    public class Conversation
    {
        [Key]
        public long ID { set; get; }
        [ForeignKey("HeaderLaporanForeignKey")]
        public BantuanHukum Header { set; get; }
        [ForeignKey("StatusBantuanForeignKey")]
        public StatusBantuan Status { set; get; }
        [ForeignKey("ParentIDForeignKey")]
        public Conversation ParentID { set; get; }
        public string Message { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ModifiedDate { set; get; }
        public string CreatedBy { set; get; }
        public bool IsRead { set; get; }
        public string FileName { set; get; }
    }
}
