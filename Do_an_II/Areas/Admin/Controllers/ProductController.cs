using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productlst = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

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
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
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

