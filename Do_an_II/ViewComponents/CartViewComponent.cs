using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Do_an_II.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SessionSettings.SessionCart) != null)
                {
                    HttpContext.Session.SetInt32(SessionSettings.SessionCart, 
                    _unitOfWork.Cart.GetAll(u => u.AppUserId == claim.Value).Count());
                }
                HttpContext.Session.SetInt32(SessionSettings.SessionCart,
                    _unitOfWork.Cart.GetAll(u => u.AppUserId == claim.Value).Count());
                return View(HttpContext.Session.GetInt32(SessionSettings.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
      
    }
    
}
