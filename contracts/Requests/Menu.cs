using System;
namespace contracts.Requests
{
    public class MenuRequest
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public int ParentMenu { set; get; }
        public string Description { set; get; }
        public bool IsMobile { set; get; }
        public string UISRef { set; get; }
        public string Icon { set; get; }
        public string Css { set; get; }
        public bool IsFolder { set; get; }
        public string UserLogin { set; get; }
    }

    public class MenuRoleRequest{
        public string RoleId { set; get; }
        public int Menu { set; get; }
    }


}
