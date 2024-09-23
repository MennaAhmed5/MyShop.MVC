using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyShop.DataAccess;
using MyShop.DataAccess.Implementations;
using MyShop.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using MyShop.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using MyShop.Entities.Models;
using MyShop.DataAccess.DbIntializer;


namespace MyShop.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
            )) ;

            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options=>options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4)
                ).AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbIntializer, DbIntializer>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();
           // SeedDb();
            app.UseAuthorization();

            app.UseSession();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}"
            );
            app.MapControllerRoute(
               name: "customer",
               pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
           );

            app.Run();

            void SeedDb()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbIntializer = scope.ServiceProvider.GetRequiredService<IDbIntializer>();
                    dbIntializer.intialize();
                }
            }
        }
    }
}
