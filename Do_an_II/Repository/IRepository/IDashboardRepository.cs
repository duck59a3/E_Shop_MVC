namespace Do_an_II.Repository.IRepository
{
    public interface IDashboardRepository
    {
        int GetTotalOrdersToday();
        double GetTotalRevenueToday();
        int GetTotalCustomersToday();
        int GetTotalOrders();
        double GetTotalRevenue();
    }
}
