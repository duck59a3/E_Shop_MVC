using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Security.Claims;

namespace Do_an_II.Areas.Customer.Controllers
{
    public class ReviewController : Controller
    {
        [Area("Customer")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Review(int productId)
        {

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult PostReview(Review rv)
        {
            return Json(true);
        }
    }
}
