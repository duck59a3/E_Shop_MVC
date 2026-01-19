using Do_an_II.Models;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = Roles.Admin)]
    public class VoucherController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VoucherController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Voucher> vouchers = _unitOfWork.Voucher.GetAll().ToList();
            return View(vouchers);
        }
    }
}
