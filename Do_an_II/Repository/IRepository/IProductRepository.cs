using Do_an_II.Models;

namespace Do_an_II.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
