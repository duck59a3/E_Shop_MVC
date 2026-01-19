namespace Do_an_II.Messagings.Messages
{
    public class OrderFailed
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
    }
}
