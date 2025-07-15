using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var totalProducts = _unitOfWork.Product.GetAll().Count();
            var totalSold = _unitOfWork.OrderDetail.GetAll()
                              .Sum(od => od.Count);

            var viewModel = new DashboardVM
            {
                TotalProducts = totalProducts,
                TotalSoldProducts = totalSold
            };

            return View(viewModel);
            
        }
    }
}
