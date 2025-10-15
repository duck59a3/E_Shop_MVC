namespace Do_an_II.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalProducts { get; set; }
        public int TotalSoldProducts { get; set; }
        public double TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public double TotalRevenueToday { get; set; }
        public int TotalOrdersToday { get; set; }
        public List<TopRegions> TotalOrdersByRegion { get; set; } = new ();

        public List<MonthlyRevenue> RevenueByMonth { get; set; } = new();
        public List<TopProductVM> TopProducts { get; set; } = new();
    }
}
