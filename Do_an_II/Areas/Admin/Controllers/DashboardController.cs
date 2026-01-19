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
                
                TotalCustomers = totalCustomers,
               
                TopProducts = topProducts,
                TotalOrdersByRegion = topRegions

            };

            return View(viewModel);

        }
        public IActionResult DoanhThu()
        {
            var totalRevenue = _unitOfWork.OrderDetail.GetAll().Sum(o => o.Price * o.Count);
            var totalRevenueToday = _unitOfWork.OrderDetail.GetAll(includeProperties: "Order")
                                    .Where(o => o.Order.OrderDate.Date == DateTime.Now.Date)
                                    .Sum(o => o.Price * o.Count);
            var totalOrders = _unitOfWork.Order.GetAll().Count();
            var totalOrdersToday = _unitOfWork.Order.GetAll()
                                    .Where(o => o.OrderDate.Date == DateTime.Now.Date)
                                    .Count();
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
            var revenuebyCategory = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product,Product.Category")
                        .GroupBy(od => od.Product.Category.Name)
                        .Select(g => new CategoryRevenue
                        {
                            CategoryName = g.Key,
                            Revenue = g.Sum(x => x.Count * x.Price)
                        })
                        .OrderByDescending(g => g.Revenue)
                        .ToList();
            var doanhThu = new RevenueVM
            {   TotalRevenueToday = totalRevenueToday,
                TotalOrdersToday = totalOrdersToday,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                RevenueByMonth = revenueByMonth,
                RevenueByCategory = revenuebyCategory

            };
            return View(doanhThu);
        }
        public IActionResult KhachHang()
        {

            var customers = _unitOfWork.AppUser.GetAll().Select(u => new CustomerReportVM
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                TotalOrders = u.TotalOrders,
                TotalSpent = u.TotalSpent,
                MemberLevel = u.MemberLevel
            }).ToList();
            var levelCounts = customers.GroupBy(c => c.MemberLevel)
                                   .ToDictionary(g => g.Key, g => g.Count());

            var ordersPerCustomer = customers
           .OrderByDescending(c => c.TotalOrders)
           .ToList();
            // 3. Pareto 80/20 khách hàng
            var totalRevenue = customers.Sum(c => c.TotalSpent);
            var paretoData = customers
                .OrderByDescending(c => c.TotalSpent)
                .ToList();

            double cumulative = 0.0;
            var paretoLabels = new List<string>();
            var paretoRevenue = new List<double>();
            var paretoCumulativePercent = new List<double>();

            foreach (var c in paretoData)
            {
                cumulative += c.TotalSpent;
                paretoLabels.Add(c.Name);
                paretoRevenue.Add(c.TotalSpent);
                paretoCumulativePercent.Add(cumulative / totalRevenue * 100);
            }
            ViewBag.LevelCounts = levelCounts;
            ViewBag.OrdersPerCustomer = ordersPerCustomer;
            ViewBag.ParetoLabels = paretoLabels;
            ViewBag.ParetoRevenue = paretoRevenue;
            ViewBag.ParetoCumulativePercent = paretoCumulativePercent;
            var customerReport = new CustomerReportVM
            {

            };
            return View(customerReport);
        }
    }
}
