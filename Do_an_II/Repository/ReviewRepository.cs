using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Review> GetReviewsByProductId(int productId)
        {
            var reviews = _db.Reviews.Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt).ToList();
            return reviews;
        }

        public void Update(Review review)
        {
            _db.Reviews.Update(review);
        }
    }
    
}
