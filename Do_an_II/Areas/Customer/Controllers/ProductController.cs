using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
       

        public IActionResult Index(int? page)
        {
            //int pageSize = 6; // Số sản phẩm trên mỗi trang
            //int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1 nếu không có tham số

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"ProductImages");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            Cart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId,includeProperties:"ProductImages"),
                ProductId = productId,
                Count = 1
            };
           
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.AppUserId = userId;
            var productFromDb = _unitOfWork.Product.Get(u => u.Id == cart.ProductId, true);
            if (productFromDb == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra vượt quá tồn kho
            if (cart.Count > productFromDb.Quantity)
            {
                TempData["error"] = $"Số lượng yêu cầu ({cart.Count}) vượt quá tồn kho hiện có ({productFromDb.Quantity}).";
                return RedirectToAction(nameof(Index));
            }
            Cart cartFromDb = _unitOfWork.Cart.Get(u => u.AppUserId == userId && u.ProductId == cart.ProductId);
            if (cartFromDb == null)
            {
                //exist
                _unitOfWork.Cart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SessionSettings.SessionCart,
                _unitOfWork.Cart.GetAll(u => u.AppUserId == userId).Count());
            }
            else
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.Cart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            productFromDb.Quantity -= cart.Count;
            _unitOfWork.Product.Update(productFromDb);
            TempData["success"] = "Thêm vào giỏ hàng thành công";
            
            return RedirectToAction(nameof(Index));
        }

 
    }
}
