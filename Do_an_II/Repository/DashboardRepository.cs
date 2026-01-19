using Do_an_II.Data;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;

namespace Do_an_II.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _db;
        public DashboardRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public int GetTotalCustomersToday()
        {
            throw new NotImplementedException();
        }

        public int GetTotalOrders()
        {
            return _db.Orders.Count();
        }

        public int GetTotalOrdersToday()
        {
            return _db.Orders.Count(o => o.OrderDate.Date == DateTime.Today);
        }

        public double GetTotalRevenue()
        {
            return _db.Orders.Sum(o => o.OrderTotal);
        }

        public double GetTotalRevenueToday()
        {
            return _db.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .Sum(o => o.OrderTotal);
        }
        
    }
}
