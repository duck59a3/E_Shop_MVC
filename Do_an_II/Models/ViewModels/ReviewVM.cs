using System.ComponentModel.DataAnnotations;

namespace Do_an_II.Models.ViewModels
{
    public class ReviewVM
    {
        public int ProductId { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
