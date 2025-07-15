using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Do_an_II.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                throw new Exception(ex.Message);
            }
            //tao role neu no khong duoc tao
            if (!_roleManager.RoleExistsAsync(Roles.Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Admin)).GetAwaiter().GetResult();

                //neu roles khong duoc tao, tao admin user
                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admink59a3@gmail.com",
                    Email = "admink59a3@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "0123456789",
                    Address = "123 Main St",
                    State = "Viet Nam",
                    PostalCode = "100000",
                    City = "Hanoi"
                }, "Admin@123").GetAwaiter().GetResult();
                AppUser user = _db.AppUsers.FirstOrDefault(u => u.Email == "admink59a3@gmail.com");
                _userManager.AddToRoleAsync(user, Roles.Admin).GetAwaiter().GetResult();
            }
            return;


        }
    }
}
