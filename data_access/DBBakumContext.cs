using System;
using Microsoft.EntityFrameworkCore;
using data_access.entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace data_access
{
    
    public class DBBakumContext:IdentityDbContext<UserProfile>
    {
        public DBBakumContext(DbContextOptions<DBBakumContext> options):base(options){
            
        }   

        public DbSet<BantuanHukum> BantuanHukum { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SessionLog> Sessions { set; get; }
        public DbSet<Conversation> Conversations { set; get; }
        public DbSet<StatusBantuan> StatusBantuan { set; get; }
        public DbSet<Template> Templates { set; get; }
        public DbSet<LogEmail> LogEmails { set; get; }
        public DbSet<Menu> Menus { set; get; }
        public DbSet<MenuRoles> MenuRoles { set; get; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
       
    }
}
