using Do_an_II.Messagings.Messages;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.RedisServices;
using Do_an_II.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Do_an_II.Services.RabbitMQServices
{
    public class FlashSaleOrderConfirmedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqConnection _connection;
        private readonly ILogger<FlashSaleOrderConfirmedConsumer> _logger;
        private IChannel _channel;
        private IConnection _conn;
        public FlashSaleOrderConfirmedConsumer(
            IServiceScopeFactory scopeFactory,
            RabbitMqConnection connection,
            ILogger<FlashSaleOrderConfirmedConsumer> logger
            )
        {
            _scopeFactory = scopeFactory;
            _connection = connection;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _conn = await _connection.CreateConnectionAsync();
            _channel = await _conn.CreateChannelAsync();

            // Xử lý từng message → tránh race condition
            await _channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    await ProcessMessageAsync(ea, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OrderConfirmed processing failed");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "order.confirmed.flashsale.queue",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation("OrderConfirmedConsumer started");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessMessageAsync(
            BasicDeliverEventArgs ea,
            CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<OrderConfirmed>(body);

            if (message == null)
            {
                _logger.LogWarning("Invalid OrderConfirmed message");
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                return;
            }

            var flashSaleItems = message.Items
                .Where(i => i.IsFlashSale)
                .ToList();

            if (!flashSaleItems.Any())
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                return;
            }

            foreach (var item in flashSaleItems)
            {
                // 1️⃣ Lấy flash sale id
                var flashSaleId = await cache.GetActiveFlashSaleIdAsync(item.ProductId);

                if (string.IsNullOrEmpty(flashSaleId))
                {
                    _logger.LogWarning(
                        "No active flash sale found for ProductId {ProductId}",
                        item.ProductId);
                    continue;
                }

                // 2️⃣ COMMIT stock (trừ stock thật)
                await cache.DecreaseFlashSaleStockAsync(
                    flashSaleId,
                    item.ProductId,
                    item.Quantity);

                
                var endTime = await cache.GetFlashSaleEndTimeAsync(flashSaleId, item.ProductId)
                              ?? DateTimeOffset.UtcNow.AddDays(1);
                // 3️⃣ Mark user đã mua flash sale
                await cache.TryMarkUserPurchasedAsync(
                    flashSaleId,
                    item.ProductId,
                    message.UserId,
                    endTime);

                // 4️⃣ Xóa reservation
                await cache.RemoveFlashSaleReservationAsync(
                    flashSaleId,
                    item.ProductId,
                    message.OrderId);

                unitOfWork.Order.UpdateStatus(
                    message.OrderId,
                    Status.StatusApproved);
                unitOfWork.Save();
                _logger.LogInformation(
                    "OrderConfirmed committed | OrderId: {OrderId}, ProductId: {ProductId}, Qty: {Qty}",
                    message.OrderId, item.ProductId, item.Quantity);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                await _channel.CloseAsync(cancellationToken);
                _channel.Dispose();
            }

            if (_conn != null)
            {
                await _conn.CloseAsync(cancellationToken);
                _conn.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
