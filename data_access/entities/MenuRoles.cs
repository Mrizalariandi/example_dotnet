using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data_access.entities
{
    public class MenuRoles
    {
        [Key]
        public int Id { set; get; }
        [ForeignKey("FKParentMenu")]
        public Menu Menu { set; get; }
        [ForeignKey("FKRole")]
        public Microsoft.AspNetCore.Identity.IdentityRole Role { set; get; }

    }
}
