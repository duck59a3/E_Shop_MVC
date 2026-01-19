namespace Do_an_II.Services.RedisServices
{
    public interface IRedisService
    {
        void SetData<T>(string key, T value, TimeSpan? expiry = null);
        T GetData<T>(string key);
        void Remove(string key);

        // ====== FLASH SALE ======

        /// <summary>
        /// Khởi tạo tồn kho flash sale cho sản phẩm
        /// </summary>
        Task<bool> InitFlashSaleStockAsync(string flashsaleId,int productId, int quantity, TimeSpan expiry);

        /// <summary>
        /// Reserve 1 sản phẩm (atomic, chống oversell)
        /// return: true nếu reserve thành công
        /// </summary>
        Task<bool> ReserveFlashSaleStockAsync(string flashsaleId, int productId);

        /// <summary>
        /// Trả lại stock khi order fail
        /// </summary>
        Task ReleaseFlashSaleStockAsync(string flashsaleId, int productId);

        /// <summary>
        /// Lấy số lượng tồn kho hiện tại
        /// </summary>
        Task<int> GetFlashSaleStockAsync(string flashsaleId, int productId);

        /// <summary>
        /// Đánh dấu user đã mua flash sale (chống spam)
        /// </summary>
        Task<bool> TryMarkUserPurchasedAsync(string flashsaleId,int productId, string userId, DateTimeOffset endtime);
        Task<bool> HasUserBoughtFlashSaleAsync(string flashsaleId,int productId, string userId);
        Task<decimal?> GetFlashSalePriceAsync(string flashsaleId, int productId);
        Task SetFlashSalePriceAsync(string flashsaleId, int productId, decimal price, TimeSpan expiry);
        Task SetFlashSaleEndTimeAsync(string flashsaleId, int productId, DateTimeOffset time, TimeSpan expiry);
        Task<DateTimeOffset?> GetFlashSaleEndTimeAsync(string flashsaleId, int productId);
        Task RemoveFlashSaleAsync(int productId, string flashsaleId);
        Task<bool> DecreaseFlashSaleStockAsync(string flashsaleId, int productId, int quantity);
        Task<string?> GetActiveFlashSaleIdAsync(int productId);
        Task SetReservationTTLAsync(string flashSaleId,int productId,int orderId, int quantity,TimeSpan ttl);

        Task RemoveFlashSaleReservationAsync(string flashSaleId,int productId, int orderId);
    }
}
