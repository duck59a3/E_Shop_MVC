using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;
using System.Linq.Expressions;

namespace Do_an_II.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Notification notification)
        {
            _db.Notifications.Update(notification);
        }

    }  
}
