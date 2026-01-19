using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Models.Dto;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.RedisServices;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _cache;
        public CategoryController(IUnitOfWork db,IRedisService redisService)
        {
            _unitOfWork = db;
           
            _cache = redisService;
        }

        //public IActionResult Index()
        //{
        //    var cacheKey = "CategoryListCache";
        //    if (!_memoryCache.TryGetValue(cacheKey, out List<Category> categorylst))
        //    {
        //        // If cache is not available, fetch data from the database
        //        categorylst = _unitOfWork.Category.GetAll().ToList();
        //        // Set cache options
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes
        //        // Save data in cache
        //        _memoryCache.Set(cacheKey, categorylst, cacheEntryOptions);
        //    }
        //    //List<Category> categorylst = _unitOfWork.Category.GetAll().ToList();
        //    return View(categorylst);
        //}

        public IActionResult Index()
        {
            var sw = Stopwatch.StartNew();
            var categoryLst = _cache.GetData<IEnumerable<Category>>("categories");
            Console.WriteLine($"Lấy dữ liệu từ cache {sw.ElapsedMilliseconds} ms");
            sw.Stop();
            if (categoryLst == null)
            {
                sw.Restart();
                categoryLst = _unitOfWork.Category.GetAll().ToList();
                _cache.SetData("categories", categoryLst, TimeSpan.FromMinutes(5));
                Console.WriteLine($"Lấy dữ liệu từ DB và lưu vào cache {sw.ElapsedMilliseconds} ms");
                sw.Stop();
            }
           
            return View(categoryLst);
            
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Thêm thành công";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            /*
            Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();*/

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Sửa thành công";

                return RedirectToAction("Index");
            }
            return View();
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
        //    /*
        //    Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        //    Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();*/

        //    if (categoryFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(categoryFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Category? category = _unitOfWork.Category.Get(u => u.Id == id);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Category.Remove(category);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Sửa thành công";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categorylst = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categorylst });
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int? id)
        {
            var category = _unitOfWork.Category.Get(u => u.Id == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Lỗi khi thực hiện xóa" });
            }
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa loại hàng thành công" });
        }
        #endregion
    }
}
