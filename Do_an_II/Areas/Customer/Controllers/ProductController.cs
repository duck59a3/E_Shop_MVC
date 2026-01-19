using Do_an_II.Models;
using Do_an_II.Models.Dto;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.RedisServices;
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
        private readonly IRedisService _cache;

        public ProductController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IRedisService cache)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }


        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 6)
        {
            
            var productList = _cache.GetData<IEnumerable<ProductDto>>("products");
            if (productList == null)
            {
                productList = _unitOfWork.Product
                   .GetAll(includeProperties: "ProductImages")
                   .Select(p => new ProductDto
                    {
                      Id = p.Id,
                      Name = p.Name,
                      Price = p.Price,
         
                      ProductImages = p.ProductImages.Select(pi => new ProductImageDto
                      {
                          Id = pi.Id,
                          ImageUrl = pi.ImageUrl,
                          
                      }).ToList()


                   }).ToList();

                _cache.SetData("products", productList, TimeSpan.FromMinutes(5));
            }

            foreach (var product in productList)
            {
                var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(product.Id);

                var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId, product.Id);
                var flashEndTime = await _cache.GetFlashSaleEndTimeAsync(flashSaleId, product.Id);

                product.FlashSalePrice = flashPrice;
                product.FlashSaleEndTime = flashEndTime;
            }
            // Tổng số sản phẩm
            int totalProducts = productList.Count();

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Lấy sản phẩm của trang hiện tại
            var products = productList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tạo ViewModel cho phân trang
            var viewModel = new ProductListVM
            {
                Products = products,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(viewModel);
        }
        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.Get(
            u => u.Id == productId,
            includeProperties: "ProductImages");



            Cart cart = new()
            {
                Product = product,
                ProductId = productId,
                Count = 1
            };

            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(Cart cart)
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
            var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(cart.ProductId);
            var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId,cart.ProductId);
            var flashQty = await _cache.GetFlashSaleStockAsync(flashSaleId, cart.ProductId);
            var flashEndTime = await _cache.GetFlashSaleEndTimeAsync(flashSaleId, cart.ProductId);
            
            bool isFlashSaleActive = flashSaleId != null;
            if (isFlashSaleActive)
            {
                // Kiểm tra user đã mua Flash Sale sản phẩm này chưa
                bool alreadyBought = await _cache.HasUserBoughtFlashSaleAsync(flashSaleId ,cart.ProductId, userId);
                if (alreadyBought)
                {
                    TempData["error"] = "Bạn chỉ được mua 1 sản phẩm Flash Sale này!";
                    return RedirectToAction(nameof(Index));
                }
                // Nếu là Flash Sale, kiểm tra tồn kho Flash Sale
                if (cart.Count > flashQty)
                {
                    TempData["error"] = $"Chỉ còn {flashQty} sản phẩm Flash Sale!";
                    return RedirectToAction(nameof(Index));
                }
                //bool reserved = await _cache.ReserveFlashSaleStockAsync(cart.ProductId);
                //if (!reserved)
                //{
                //    TempData["error"] = "Sản phẩm Flash Sale đã hết!";
                //    return RedirectToAction(nameof(Index));
                //}
                //await _cache.TryMarkUserPurchasedAsync(cart.ProductId, userId); // đánh dấu đã mua
                cart.Price = (double)flashPrice.Value;
            }

            else
            {
                // Nếu không phải Flash Sale, kiểm tra tồn kho bình thường
                // Kiểm tra vượt quá tồn kho
                if (cart.Count > productFromDb.Quantity)
                {
                    TempData["error"] = $"Số lượng yêu cầu ({cart.Count}) vượt quá tồn kho hiện có ({productFromDb.Quantity}).";
                    
                    return RedirectToAction(nameof(Index));
                }
                cart.Price = (double)productFromDb.Price;
            }
            Cart cartFromDb = _unitOfWork.Cart.Get(u => u.AppUserId == userId && u.ProductId == cart.ProductId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                //cartFromDb.Price = cart.Price;
                _unitOfWork.Cart.Update(cartFromDb);
                _unitOfWork.Save();
                
            }
            else
            {
                //chưa có cart thì thêm
                cart.Price = isFlashSaleActive
                ? (double)flashPrice.Value
                : (double)productFromDb.Price;
                _unitOfWork.Cart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SessionSettings.SessionCart,
                _unitOfWork.Cart.GetAll(u => u.AppUserId == userId).Count());
            }
            if (!isFlashSaleActive)
            {
                productFromDb.Quantity -= cart.Count;
                _unitOfWork.Product.Update(productFromDb);
            }
            TempData["success"] = "Thêm vào giỏ hàng thành công";

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet("search")]
        public IActionResult SearchProduct(string query)
        {
            var products = _unitOfWork.Product.Search(query);
            return Json(products);

        }
        #endregion

    }
}
