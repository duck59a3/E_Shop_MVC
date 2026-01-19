using System.ComponentModel.DataAnnotations.Schema;

namespace Do_an_II.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUser { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
