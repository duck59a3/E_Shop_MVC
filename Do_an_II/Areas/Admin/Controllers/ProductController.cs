using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.RedisServices;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRedisService _redisService;
        private readonly IDatabase _db;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment, IRedisService redisService, IConnectionMultiplexer redis)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
            _redisService = redisService;
            _db = redis.GetDatabase();
        }
        public async Task<IActionResult> Index()
        {
            List<Product> productlst = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            var flashSaleMap = new Dictionary<int, decimal?>();

            foreach (var p in productlst)
            {
                var flashSaleId = await _redisService.GetActiveFlashSaleIdAsync(p.Id);
                if (!string.IsNullOrEmpty(flashSaleId))
                {
                    var price = await _redisService.GetFlashSalePriceAsync(flashSaleId, p.Id);
                    flashSaleMap[p.Id] = price;
                }
            }

            ViewBag.FlashSaleMap = flashSaleMap;

            return View(productlst);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                //create product
                return View(productVM);
            }
            else
            {
                //update product
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages");
                return View(productVM);
            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {


            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    // Create new product
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Thêm thành công";
                }
                else
                {
                    // Update existing product
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Cập nhật thành công";
                }
                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"img\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ProductId = productVM.Product.Id,
                            ImageUrl = @"\" + productPath + @"\" + fileName
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }
                        productVM.Product.ProductImages.Add(productImage);

                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();






                }
                TempData["success"] = "Sửa thành công";
                return RedirectToAction("Index");
            }
            else
            {
                // Gán lại CategoryList nếu form bị lỗi
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(productVM);

            }

        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageDelete = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageDelete.ProductId;
            if (imageDelete != null)
            {
                if(!string.IsNullOrEmpty(imageDelete.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageDelete.ImageUrl.TrimStart('\\'));
                    
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imageDelete);
                _unitOfWork.Save();
                TempData["success"] = "Xóa ảnh thành công";
            }
            return RedirectToAction(nameof(Upsert),new {id= productId});
        }

        [HttpGet]
        public IActionResult FlashSale(int id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null) return NotFound();

            return View(new FlashSaleVM
            {
                ProductId = product.Id,
                FlashPrice = product.Price,
                Quantity = 10,
                DurationMinutes = 60
            });
        }
        [HttpPost]
        public IActionResult FlashSale(FlashSaleVM vm)
        {
            var duration = TimeSpan.FromMinutes(vm.DurationMinutes);
            var flashSaleEndTime = DateTimeOffset.UtcNow.Add(duration);
            string flashSaleId = Guid.NewGuid().ToString();
            _db.SetAddAsync("flashsale:active", flashSaleId);
            _db.SetAddAsync($"flashsale:{flashSaleId}:products", vm.ProductId);
            _redisService.SetFlashSalePriceAsync(flashSaleId,vm.ProductId, vm.FlashPrice, duration);

            _redisService.InitFlashSaleStockAsync(flashSaleId, vm.ProductId, vm.Quantity, duration);
            _redisService.SetFlashSaleEndTimeAsync(flashSaleId, vm.ProductId, flashSaleEndTime, duration);
            
            TempData["success"] = "Đã bật Flash Sale";
            return RedirectToAction(nameof(Index));
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category")
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    CategoryName = p.Category.Name,
                })
                .ToList();
            var productList = new List<object>();

            foreach (var p in products)
            {
                var flashSaleId = await _redisService.GetActiveFlashSaleIdAsync(p.Id);

                bool isFlashSale = !string.IsNullOrEmpty(flashSaleId);

                decimal? flashPrice = null;
                int? flashStock = null;
                if (isFlashSale)
                {
                    flashPrice = await _redisService.GetFlashSalePriceAsync(flashSaleId, p.Id);
                    flashStock = await _redisService.GetFlashSaleStockAsync(flashSaleId, p.Id);
                }
                
                
                
                

                productList.Add(new
                {
                    p.Id,
                    p.Name,
                    Price = isFlashSale ? flashPrice : p.Price,
                    Quantity = isFlashSale ? flashStock : p.Quantity,
                    p.CategoryName,
                    IsFlashSale = isFlashSale
                });
            }
            return Json(new { data = productList });
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
           var productDelete = _unitOfWork.Product.Get(u => u.Id == id);
            if (productDelete == null)
            {
                return Json(new { success = false, message = "Lỗi không tìm thấy sản phẩm" });
            }
            string productPath = @"img\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            if (!Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (var filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }
            _unitOfWork.Product.Remove(productDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa sản phẩm thành công" });
        }
        #endregion
    }
}

