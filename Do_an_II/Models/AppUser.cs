using Do_an_II.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Do_an_II.Models
{
    public class AppUser : IdentityUser 
    {
        [Required]
        public string Name { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public int TotalOrders { get; set; }
        public double TotalSpent { get; set; }
        [NotMapped]
        public MemberLevel MemberLevel { get; set; } = MemberLevel.Bronze;
        [ValidateNever]
        public ICollection<VoucherUsage> VoucherUsages { get; set; }

    }
}
