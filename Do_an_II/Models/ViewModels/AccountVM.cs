namespace Do_an_II.Models.ViewModels
{
    public class AccountVM
    {
        public AppUser AppUser { get; set; }
        public IEnumerable<OrderVM> Orders { get; set; }
        public IEnumerable<VoucherVM> Vouchers { get; set; }
    }
}
