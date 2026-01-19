using Do_an_II.Models;

namespace Do_an_II.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        void Update(Review review);
        IEnumerable<Review> GetReviewsByProductId(int productId);
    }
    
}
