namespace Do_an_II.Messagings.Messages
{
    public class OrderItemMessage
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsFlashSale { get; set; }
        public decimal Price { get; set; }
        public string FlashSaleId { get; set; }
    }
}
