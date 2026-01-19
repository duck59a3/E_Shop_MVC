namespace Do_an_II.Models.ViewModels
{
    public class RevenueVM
    {
        public double TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenueToday { get; set; }
        public int TotalOrdersToday { get; set; }
        public List<MonthlyRevenue> RevenueByMonth { get; set; } = new();
        public List<CategoryRevenue> RevenueByCategory { get; set; } = new();
    }
}
