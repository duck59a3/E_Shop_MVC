using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository;
using Do_an_II.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    [Authorize]
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public OrderVM OrderVM { get; set; }
        public AccountController(ApplicationDbContext db, ILogger<AccountController> logger, IUnitOfWork unitOfWork)
        {
            _db = db;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            var orders = _unitOfWork.Order.GetAll(
                  u => u.AppUser.Id == userId, includeProperties: "AppUser,OrderDetails,OrderDetails.Product");
            var vouchers = _unitOfWork.VoucherUsage.GetAll(u => u.UserId == userId, includeProperties: "Voucher");
            var viewModel = new AccountVM
            {
                AppUser = user,
                Orders = orders.Select(order => new OrderVM
                {
                    Order = order,
                    OrderDetails = order.OrderDetails
                }).ToList(),
                Vouchers = vouchers.Select(voucherUsage => new VoucherVM
                {
                    Id = voucherUsage.Id,
                    Code = voucherUsage.Voucher.Code,
                    DiscountType = voucherUsage.Voucher.DiscountType,
                    DiscountValue = voucherUsage.Voucher.DiscountValue,
                    ExpiryDate = voucherUsage.Voucher.EndDate,
                    IsUsed = voucherUsage.Voucher.IsActive
                }).ToList()
            };



            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateShippingAddress(string address, string phone)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Address = address;
            user.PhoneNumber = phone;
            _unitOfWork.AppUser.Update(user);
            _unitOfWork.Save();
            TempData["success"] = "Cập nhật địa chỉ giao hàng thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(AppUser user)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromDb = _unitOfWork.AppUser.Get(u => u.Id == userId);
            if (userFromDb == null)
            {
                return NotFound();
            }
            userFromDb.Name = user.Name;
            userFromDb.Address = user.Address;
            userFromDb.PhoneNumber = user.PhoneNumber;
            userFromDb.Email = user.Email;
            _unitOfWork.AppUser.Update(userFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Cập nhật thông tin cá nhân thành công";
            return RedirectToAction("Index");

        }
    }
}
