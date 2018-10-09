using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data_access.entities
{
    public class Menu
    {
        [Key]
        public int Id { set; get; }
        public string Title { set; get; }
        [ForeignKey("FKParentMenu")]
        public Menu ParentMenu { set; get; }
        public string Description { set; get; }
        public bool IsMobile { set; get; }
        public string UISRef { set; get; }
        public string Icon { set; get; }
        public string Css { set; get; }
        public bool IsFolder { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ModifiedDate { set; get; }
        public string CreatedBy { set; get; }
        public string ModifiedBy { set; get; }
    
    }
}
