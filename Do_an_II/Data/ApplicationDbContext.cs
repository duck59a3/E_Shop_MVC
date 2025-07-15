using Do_an_II.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Do_an_II.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    DisplayOrder = 1,
                    Name = "Áo",
                    Description = "Áo thời trang"
                },
                new Category
                {
                    Id = 2,
                    DisplayOrder = 2,
                    Name = "Quần dài",
                    Description = "Quần thời trang"
                }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Áo thun",
                    Description = "Áo thun nam",
                    Price = 100,
                    Quantity = 10,
                    Size = "M",
                    Color = "Đen",
                    Material = "Cotton",
                    CategoryId = 7
                },
                new Product
                {
                    Id = 2,
                    Name = "Áo sơ mi",
                    Description = "Áo sơ mi nam",
                    Price = 200,
                    Quantity = 20,
                    Size = "L",
                    Color = "Trắng",
                    Material = "Vải",
                    CategoryId = 9
                });




        }
    }
}
