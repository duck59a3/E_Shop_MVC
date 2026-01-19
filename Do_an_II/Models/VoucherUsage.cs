using System.ComponentModel.DataAnnotations.Schema;

namespace Do_an_II.Models
{
    public class VoucherUsage
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public double DiscountAmount { get; set; } // Số tiền đã giảm
        public DateTime UsedDate { get; set; }

        // Navigation
        [ForeignKey("VoucherId")]
        public Voucher Voucher { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [ForeignKey("OrderId")]
        public  Order Order { get; set; }
    }
}
