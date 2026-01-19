namespace Do_an_II.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public decimal? FlashSalePrice { get; set; } // Giá flash
        public DateTimeOffset? FlashSaleEndTime { get; set; } // Thời gian kết thúc flash sale
        public List<ProductImageDto> ProductImages { get; set; }
        public bool IsFlashSale => FlashSalePrice.HasValue;
    }
}
