namespace Do_an_II.Messagings.Messages
{
    public record OrderFlashSale(
        int orderId,
        int userId,
        int productId,
        int quantity,
        double price);

}
