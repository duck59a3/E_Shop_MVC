using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Do_an_II.Models
{
    public class Product
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; } 
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Size { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Color { get; set; }

        [Required]  
        [Column(TypeName = "nvarchar(100)")]
        public string Material { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Navigation properties
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
        [ValidateNever]
        public List<Review> Reviews { get; set; }


        }
}

