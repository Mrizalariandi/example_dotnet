using System;
using System.ComponentModel.DataAnnotations;

namespace data_access.entities
{
    public class SessionLog
    {
        [Key]
        public long ID { set; get; }
        public string UserName { set; get; }
        public string Token { set; get; }
        public DateTime DateCreate { set; get; }
        public DateTime DateEnd { set; get; }
        public string IP { set; get; }
        public string Platform { set; get; }
        public string DN { set; get; }
    }
}
