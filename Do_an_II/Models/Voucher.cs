using Do_an_II.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Do_an_II.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; } // Mã voucher
        public string Name { get; set; }
        public string Description { get; set; }

        // Loại giảm giá
        public DiscountType DiscountType { get; set; } // Percentage hoặc FixedAmount
        public double DiscountValue { get; set; }
        public double? MaxDiscountAmount { get; set; } // Giảm tối đa (cho % discount)
        public double? MinOrderAmount { get; set; } // Đơn hàng tối thiểu

        // Điều kiện áp dụng
        public MemberLevel MinimumTier { get; set; } // Bậc thành viên tối thiểu
        public int Quantity { get; set; } // Số lượng voucher
        public int UsedQuantity { get; set; } = 0; // Đã sử dụng
        public int MaxUsagePerUser { get; set; } = 1; // Giới hạn sử dụng/người

        // Thời gian
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate && UsedQuantity < Quantity;

        // Navigation
        public ICollection<VoucherUsage> VoucherUsages { get; set; }
    }
}
