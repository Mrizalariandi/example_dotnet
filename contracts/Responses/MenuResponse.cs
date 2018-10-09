using System;
namespace contracts.Responses
{
    public class MenuResponse
    {
        public string Title { set; get; }
        public int ParentMenu { set; get; }
        public string Description { set; get; }
        public bool IsMobile { set; get; }
        public string UISRef { set; get; }
        public string Icon { set; get; }
        public string Css { set; get; }
        public bool IsFolder { set; get; }
        public MenuResponse[] Children { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ModifiedDate { set; get; }
        public string CreatedBy { set; get; }
        public string ModifiedBy { set; get; }
    
    }
}
