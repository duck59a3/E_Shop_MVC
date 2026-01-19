using Do_an_II.Utilities;

namespace Do_an_II.Models.ViewModels
{
    public class VoucherVM
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public DiscountType DiscountType { get; set; }   // Percent / Cash
        public double DiscountValue { get; set; }

        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
    }
}
