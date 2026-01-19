using Do_an_II.Models.Dto;

namespace Do_an_II.Models.ViewModels
{
    public class ProductListVM
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
