using System;
using System.ComponentModel.DataAnnotations;

namespace data_access.entities
{
    public class Template
    {
        [Key]
        public Int16 ID { set; get; }
        public string TemplateName { set; get; }
        public string Title { set; get; }
        public string Message { set; get; }
    }
}
