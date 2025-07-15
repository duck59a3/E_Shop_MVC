using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]

    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAllEmployee()
        {

            List<AppUser> userList = _db.AppUsers.ToList(); // danh sách user
            var userRoles = _db.UserRoles.ToList(); // Danh sách role của user
            var roles = _db.Roles.ToList(); // Customer, Employee, Admin
            List<AppUser> employeeList = new List<AppUser>(); // danh sách nhân viên

            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.Role == "Employee")
                {
                    employeeList.Add(user);
                }
            }

            return Json(new { data = employeeList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var userDb = _db.AppUsers.FirstOrDefault(u => u.Id == id);
            if (userDb == null)
            {
                return Json(new { success = false, message = "Không tồn tại người dùng này" });
            }
            if (userDb.LockoutEnd != null && userDb.LockoutEnd > DateTime.Now)
            {
                userDb.LockoutEnd = DateTime.Now;

            }
            else
            {
                userDb.LockoutEnd = DateTime.Now.AddYears(1000);


            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Thay đổi quyền truy cập tài khoản này thành công" });

            #endregion

        }
    }
}
