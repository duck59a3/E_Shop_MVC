namespace Do_an_II.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Cart> CartList { get; set; }
        public Order Order { get; set; }
    }
}
