using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Security.Claims;

namespace Do_an_II.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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
        public IActionResult PostReview(int productId, int rating, string comment)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(comment))
                return BadRequest("Vui lòng nhập nội dung bình luận");

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _unitOfWork.Review.Add(review);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}
