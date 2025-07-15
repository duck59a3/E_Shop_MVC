using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    [Authorize]
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public IActionResult Get()
        {
            var userName = User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            return View(user);
        }
    }
}
