namespace Do_an_II.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICartRepository Cart { get; }
        IAppUserRepository AppUser { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; }
        IProductImageRepository ProductImage { get; }
        IReviewRepository Review { get; }
        INotificationRepository Notification { get; }
        IDashboardRepository Dashboard { get; }
        void Save();
    }
}
