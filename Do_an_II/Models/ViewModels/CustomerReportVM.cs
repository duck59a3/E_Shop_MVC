using Do_an_II.Utilities;

namespace Do_an_II.Models.ViewModels
{
    public class CustomerReportVM
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TotalOrders { get; set; }
        public double TotalSpent { get; set; }
        public MemberLevel MemberLevel { get; set; }

    }
}
