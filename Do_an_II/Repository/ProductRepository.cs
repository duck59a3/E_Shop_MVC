using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(s => s.Id == product.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = product.Name;
                objFromDb.Description = product.Description;
                objFromDb.Price = product.Price;
                objFromDb.Color = product.Color;
                objFromDb.Size = product.Size;
                objFromDb.Quantity = product.Quantity;
                objFromDb.Material = product.Material;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.ProductImages = product.ProductImages; // Cập nhật danh sách ảnh sản phẩm
            }
        }
    }
}
