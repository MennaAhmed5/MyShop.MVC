using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyShop.Entities.Models;
using MyShop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.DbIntializer
{
    public class DbIntializer : IDbIntializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbIntializer(
            UserManager<ApplicationUser> userManager,           
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;            
            _roleManager = roleManager;
            _context = context;
        }
        public void intialize()
        {
            //Migration
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 1)
                {
                    _context.Database.Migrate();
                }
            }
            catch(Exception)
            {
                throw;
            }

            //Roles

          if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
          {
              _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
              _roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
              _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();

                //User

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Admin77",
                    Email ="Admin77@gmail.com",
                    Name = "Adminstrator",
                    PhoneNumber="01212314708",
                    Address = "Portfoad",
                    City = "Portsaid"
                },"Test1234####").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin55@gmail.com");
                _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
          }

            return;

        }
    }
}
