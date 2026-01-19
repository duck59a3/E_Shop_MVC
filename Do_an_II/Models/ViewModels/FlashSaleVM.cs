namespace Do_an_II.Models.ViewModels
{
    public class FlashSaleVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FlashPrice { get; set; }
        public int Quantity { get; set; }
        public int DurationMinutes { get; set; }
    }
}
