using Do_an_II.Models.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Do_an_II.Services.RedisServices
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        private readonly IDatabase _db;
        public RedisService(IDistributedCache cache, IConnectionMultiplexer redis)
        {
            _cache = cache;
            _db = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cache?.GetString(key);
            if (value is null)
            {
                return default(T);
            }
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            return JsonSerializer.Deserialize<T>(value)!;
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void SetData<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expiry
            };
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = false
            };
            _cache.SetString(key, JsonSerializer.Serialize(value), options);
        }
        // ================= FLASH SALE =================

        private string PriceKey(string flashSaleId,int productId)
            => $"flashsale:{flashSaleId}:price:{productId}";
        private string StockKey(string flashSaleId, int productId)
            => $"flashsale:{flashSaleId}:stock:{productId}";

        private string UserKey(int productId,string flashSaleId)
            => $"flashsale:{flashSaleId}:purchased:{productId}";
        private string EndTime(string flashSaleId, int productId)
            => $"flashsale:{flashSaleId}:end:{productId}";

        /// <summary>
        /// Khởi tạo tồn kho flash sale
        /// </summary>
        public async Task<bool> InitFlashSaleStockAsync(string flashsaleId,int productId, int quantity, TimeSpan expiry)
        {
            var key = StockKey(flashsaleId,productId);
            return await _db.StringSetAsync(key, quantity, expiry);
        }

        // -------- Lua Script reserve stock (ATOMIC) --------
        private const string ReserveLuaScript = @"
        local stock = tonumber(redis.call('GET', KEYS[1]) or '0')
        if stock <= 0 then
            return -1
        end
        redis.call('DECR', KEYS[1])
        return stock - 1
    ";

        /// <summary>
        /// Reserve 1 sản phẩm (atomic, chống oversell)
        /// </summary>
        public async Task<bool> ReserveFlashSaleStockAsync(string flashsaleId,int productId)
        {
            var key = StockKey(flashsaleId, productId);

            var result = (int)await _db.ScriptEvaluateAsync(
                ReserveLuaScript,
                new RedisKey[] { key }
            );

            return result >= 0;
        }

        /// <summary>
        /// Trả lại stock khi order fail
        /// </summary>
        public async Task ReleaseFlashSaleStockAsync(string flashsaleId,int productId)
        {
            var key = StockKey(flashsaleId, productId);
            await _db.StringIncrementAsync(key);
        }

        /// <summary>
        /// Lấy số lượng tồn kho hiện tại
        /// </summary>
        public async Task<int> GetFlashSaleStockAsync(string flashsaleId,int productId)
        {
            var key = StockKey(flashsaleId, productId);
            var value = await _db.StringGetAsync(key);

            return value.HasValue ? (int)value : 0;
        }

        /// <summary>
        /// Đánh dấu user đã mua flash sale (chống spam)
        /// </summary>
        public async Task<bool> TryMarkUserPurchasedAsync(string flashsaleId, int productId, string userId, DateTimeOffset endtime)
        {
            var key = UserKey(productId,flashsaleId);

            //return true nếu add mới, false nếu đã tồn tại
            var added = await _db.SetAddAsync(key, userId);

            if (added)
            {
                var ttl = endtime - DateTimeOffset.UtcNow;
                if (ttl.TotalSeconds > 0)
                {
                    await _db.KeyExpireAsync(key, ttl);
                }
            }

            return added; // true = mua lần đầu trong flash sale này
        }
        public async Task SetFlashSalePriceAsync(string flashsaleId,int productId,decimal price, TimeSpan expiry)
        {
            var key = PriceKey(flashsaleId, productId);
            await _db.StringSetAsync(key, price.ToString(), expiry);
        }

        public async Task<decimal?> GetFlashSalePriceAsync(string flashsaleId,int productId)
        {
            var value = await _db.StringGetAsync(PriceKey(flashsaleId, productId));
            return value.HasValue ? (decimal)value : null;
        }
        public async Task SetFlashSaleEndTimeAsync(string flashsaleId,int productId, DateTimeOffset time, TimeSpan expiry)
        {
            var key = EndTime(flashsaleId, productId);
            await _db.StringSetAsync(key, time.ToUnixTimeSeconds(), expiry);
        }
        public async Task<DateTimeOffset?> GetFlashSaleEndTimeAsync(string flashsaleId,int productId)
        {
            var value = await _db.StringGetAsync(EndTime(flashsaleId, productId));
            if (value.HasValue && long.TryParse(value, out long unixSeconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
            }
            return null;
        }
        public async Task<bool> HasUserBoughtFlashSaleAsync(string flashsaleId, int productId, string userId)
        {
            var key = UserKey(productId,flashsaleId);
            return await _db.SetContainsAsync(key, userId);
        }
        public async Task RemoveFlashSaleAsync(int productId, string flashsaleId)
        {
            var keys = new RedisKey[]
            {
        PriceKey(flashsaleId, productId),
        StockKey(flashsaleId, productId),
        EndTime(flashsaleId, productId),
        UserKey(productId,flashsaleId)
            };
            await _db.KeyDeleteAsync(keys);
        }
        public async Task<bool> DecreaseFlashSaleStockAsync(string flashsaleId,int productId, int quantity)
        {
            var key = StockKey(flashsaleId, productId);

            // Lua script để đảm bảo không âm
            string script = @"
        local stock = tonumber(redis.call('GET', KEYS[1]))
        if not stock or stock < tonumber(ARGV[1]) then
            return -1
        end
        return redis.call('DECRBY', KEYS[1], ARGV[1])
    ";

            var result = (long)await _db.ScriptEvaluateAsync(
                script,
                new RedisKey[] { key },
                new RedisValue[] { quantity }
            );

            return result >= 0;


        }
        public async Task<string?> GetActiveFlashSaleIdAsync(int productId)
        {
            // Set chứa các flash sale đang active
            const string activeKey = "flashsale:active";

            // Lấy tất cả flashSaleId đang active
            var flashSaleIds = await _db.SetMembersAsync(activeKey);
            if (flashSaleIds == null || flashSaleIds.Length == 0)
                return null;

            foreach (var fsIdValue in flashSaleIds)
            {
                var flashSaleId = fsIdValue.ToString();
                if (string.IsNullOrEmpty(flashSaleId))
                    continue;

                // 1️⃣ Check product có thuộc flash sale này không
                var productSetKey = $"flashsale:{flashSaleId}:products";
                bool containsProduct = await _db.SetContainsAsync(productSetKey, productId);
                if (!containsProduct)
                    continue;

                // 2️⃣ Lấy endTime từ Redis
                var endKey = $"flashsale:{flashSaleId}:end:{productId}";
                var endTimeValue = await _db.StringGetAsync(endKey);
                if (!endTimeValue.HasValue)
                    continue;

                // Parse giá trị lưu trong Redis sang long
                if (!long.TryParse(endTimeValue.ToString(), out long unixSeconds))
                    continue;

                var endTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

                // 3️⃣ Kiểm tra flash sale còn active không
                if (endTime > DateTimeOffset.UtcNow)
                {
                    // 🎯 Found active flash sale
                    return flashSaleId;
                }
            }

            return null;
        }
        /// <summary>
        /// Reserve stock + set TTL cho order
        /// Dùng trong OrderCreated
        /// </summary>
        public async Task SetReservationTTLAsync(string flashSaleId, int productId,int orderId,int quantity,TimeSpan ttl)
        {
            var reserveKey = $"flashsale:reserve:{flashSaleId}:{productId}:{orderId}";
            var reservedCounterKey = $"flashsale:reserved:{flashSaleId}:{productId}";

            // Payload để debug / audit
            var payload = new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                ReservedAt = DateTimeOffset.UtcNow
            };

            // Lua script đảm bảo atomic
            var script = @"
            redis.call('SET', KEYS[1], ARGV[1], 'EX', ARGV[2])
            redis.call('INCRBY', KEYS[2], ARGV[3])
            return 1
        ";

            await _db.ScriptEvaluateAsync(
                script,
                keys: new RedisKey[]
                {
                reserveKey,
                reservedCounterKey
                },
                values: new RedisValue[]
                {
                JsonSerializer.Serialize(payload),
                (int)ttl.TotalSeconds,
                quantity
                });
        }

        /// <summary>
        /// Remove reservation khi OrderConfirmed
        /// </summary>
        public async Task RemoveFlashSaleReservationAsync(
            string flashSaleId,
            int productId,
            int orderId)
        {
            var reserveKey = $"flashsale:reserve:{flashSaleId}:{productId}:{orderId}";
            var reservedCounterKey = $"flashsale:reserved:{flashSaleId}:{productId}";

            // Lấy quantity để rollback counter
            var value = await _db.StringGetAsync(reserveKey);
            if (value.IsNullOrEmpty)
                return;

            var payload = JsonSerializer.Deserialize<ReservationPayload>(value!);
            if (payload == null)
                return;

            var script = @"
            redis.call('DEL', KEYS[1])
            redis.call('DECRBY', KEYS[2], ARGV[1])
            return 1
        ";

            await _db.ScriptEvaluateAsync(
                script,
                keys: new RedisKey[]
                {
                reserveKey,
                reservedCounterKey
                },
                values: new RedisValue[]
                {
                payload.Quantity
                });
        }

        private class ReservationPayload
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public DateTimeOffset ReservedAt { get; set; }
        }
    }
}
