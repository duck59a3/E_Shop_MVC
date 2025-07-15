using Do_an_II.Data;
using Do_an_II.Migrations;
using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]

    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           
            return View();
        }
   
        #region API CALLS
        [HttpGet]
        public IActionResult GetAllCustomer()
        {

            List<AppUser> userList = _db.AppUsers.ToList(); // danh sách user
            var userRoles = _db.UserRoles.ToList(); // Danh sách role của user
            var roles = _db.Roles.ToList(); // Customer, Employee, Admin
            List<AppUser> customerList = new List<AppUser>(); // danh sách khach hang
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.Role == "Customer")
                {
                    customerList.Add(user);
                }







            }

            return Json(new { data = customerList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
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
        }
        [HttpDelete]
        public IActionResult Delete([FromBody]string id)
        {
            var user = _unitOfWork.AppUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Lỗi khi thực hiện xóa" });
            }
            _unitOfWork.AppUser.Remove(user);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa người dùng này thành công" });
        }
        #endregion
    }
}
