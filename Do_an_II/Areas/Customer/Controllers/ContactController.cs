using Microsoft.AspNetCore.Mvc;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    public class ContactController : Controller
    {

        public IActionResult Index()
        {

            return View();
        }
    }
}
