using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId)) {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId)) {
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
    
}
