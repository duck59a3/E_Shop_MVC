namespace Do_an_II.Messagings.Messages
{
    public class OrderCreated
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public List<OrderItemMessage> Items { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
