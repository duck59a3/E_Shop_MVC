using Do_an_II.Models;

namespace Do_an_II.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void Update(Cart cart);
    }
}
