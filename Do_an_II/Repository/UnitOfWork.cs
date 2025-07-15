using Do_an_II.Data;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category{ get; private set; }
        public IProductRepository Product { get; private set; }
        public ICartRepository Cart { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public IReviewRepository Review { get; private set; }
        public INotificationRepository Notification { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            AppUser = new AppUserRepository(_db);
            Cart = new CartRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Order = new OrderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            Review = new ReviewRepository(_db);
        }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
