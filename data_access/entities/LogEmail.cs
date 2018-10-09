using System;
using System.ComponentModel.DataAnnotations;

namespace data_access.entities
{

    public class LogEmail
    {
     
        [Key]
        public long ID { set; get; }
        public string Subject { set; get; }

        /// <summary>
        /// Email
        /// </summary>
        /// <value>To.</value>
        public string To { set; get; }
        public string Body { set; get; }
        public bool Sent { set; get; }
        public DateTime CreatedDate { set; get; }
        public string Error { set; get; }

        public bool Blocked { set; get; }
    }
}
