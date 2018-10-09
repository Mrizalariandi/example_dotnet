using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data_access;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using services;
using repositories;
using Microsoft.AspNetCore.Identity;
using contracts;
using data_access.entities;
using System.IO;

namespace api_bakum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();


            /** CORS STANDAR Without IdentityServer
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(Configuration.GetSection("AllowOriginPolicy").Value)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }); **/

              services.AddCors(options =>
            {
                options.AddPolicy("default",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            /**/

            services.Configure<LdapConfig>(Configuration.GetSection("ldap"));
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));
            services.Configure<EmailConfig>(Configuration.GetSection("EmailSettings"));

            services.AddDbContext<DBBakumContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),b=>b.MigrationsAssembly("api_bakum")));

            // Identity Role
            services.AddIdentity<UserProfile, IdentityRole>()
            .AddEntityFrameworkStores<DBBakumContext>()
              .AddDefaultTokenProviders();

            // Repository
            services.AddScoped<IBantuanHukumService, BantuanHukumRepository>();
            services.AddScoped<IAuthenticationService, LdapAuthenticationService>();


            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 2;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
               // options.User.RequireUniqueEmail = true;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,DBBakumContext context)
        {
                                    app.UseCors("default");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseMvc();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "aspnetcore_files")),
                RequestPath = "/staticFiles"
            });
        }
    }
}
