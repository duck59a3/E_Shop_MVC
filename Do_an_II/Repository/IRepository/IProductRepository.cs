using Do_an_II.Models;
using Do_an_II.Models.Dto;

namespace Do_an_II.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        List<ProductSearchDto> Search(string query);
    }
}
