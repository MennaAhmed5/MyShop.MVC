 using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop.Entities.Models;
using System.Xml;

namespace MyShop.DataAccess
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
