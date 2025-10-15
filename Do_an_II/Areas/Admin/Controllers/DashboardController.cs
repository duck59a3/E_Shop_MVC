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
            var totalProducts = _unitOfWork.Product.GetAll().Sum(s => s.Quantity);
            var totalSold = _unitOfWork.OrderDetail.GetAll()
                              .Sum(od => od.Count);

            //Doanh thu tháng
            var revenueByMonth = _unitOfWork.OrderDetail.GetAll(includeProperties: "Order")
                        .Where(o => o.Order.OrderDate.Year == DateTime.Now.Year)
                        .GroupBy(o => o.Order.OrderDate.Month)
                        .Select(g => new MonthlyRevenue
                        {
                            Month = new DateTime(DateTime.Now.Year, g.Key, 1).ToString("MMM"),
                            Revenue = g.Sum(x => x.Count * x.Price)
                        })
                        .OrderBy(g => g.Month)
                        .ToList();

            // Top 5 sản phẩm bán chạy
            var topProducts = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product")
                .GroupBy(od => od.Product.Name)
                .Select(g => new TopProductVM
                {
                    ProductName = g.Key,
                    SoldQuantity = g.Sum(x => x.Count)
                })
                .OrderByDescending(g => g.SoldQuantity)
                .Take(5)
                .ToList();

            // Top vùng miền đặt hàng
            var topRegions = _unitOfWork.Order.GetAll()
                .GroupBy(o => o.City)
                .Select(g => new TopRegions
                {
                    Region = g.Key,
                    OrderCount = g.Count()
                }).ToList();





            // Tổng doanh thu, đơn hàng, khách hàng
            var totalRevenue = _unitOfWork.OrderDetail.GetAll().Sum(o => o.Price * o.Count);
            var totalRevenueToday = _unitOfWork.OrderDetail.GetAll(includeProperties: "Order")
                                    .Where(o => o.Order.OrderDate.Date == DateTime.Now.Date)
                                    .Sum(o => o.Price * o.Count);
            
            var totalOrders = _unitOfWork.Order.GetAll().Count();
            var totalOrdersToday = _unitOfWork.Order.GetAll()
                                    .Where(o => o.OrderDate.Date == DateTime.Now.Date)
                                    .Count();
            var totalCustomers = _unitOfWork.AppUser.GetAll().Count();

            var viewModel = new DashboardVM
            {
                TotalProducts = totalProducts,
                TotalSoldProducts = totalSold,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalCustomers = totalCustomers,
                TotalOrdersToday = totalOrdersToday,
                TotalRevenueToday = totalRevenueToday,
                RevenueByMonth = revenueByMonth,
                TopProducts = topProducts,
                TotalOrdersByRegion = topRegions

            };

            return View(viewModel);
            
        }
    }
}
