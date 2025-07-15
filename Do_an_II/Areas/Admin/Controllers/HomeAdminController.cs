using Do_an_II.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Do_an_II.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class HomeAdminController : Controller
    {
        [Route("~/Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChatAdmin()
        {
            return View();
        }
    }
}
