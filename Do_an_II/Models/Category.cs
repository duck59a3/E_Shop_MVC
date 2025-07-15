using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Do_an_II.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Description { get; set; }
    }
}
