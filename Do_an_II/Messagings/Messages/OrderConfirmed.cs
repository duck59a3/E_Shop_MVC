namespace Do_an_II.Messagings.Messages
{
    public class OrderConfirmed
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentId { get; set; }
        public List<OrderItemMessage> Items { get; set; }
    }
}
