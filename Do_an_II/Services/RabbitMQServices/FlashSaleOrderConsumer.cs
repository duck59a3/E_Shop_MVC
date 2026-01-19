using Do_an_II.Messagings.Messages;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.RedisServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Do_an_II.Services.RabbitMQServices
{
    public class FlashSaleOrderConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqConnection _connection;
        private readonly ILogger<FlashSaleOrderConsumer> _logger;
        private IChannel _channel;
        private IConnection _conn;
        private static readonly string InstanceId = Guid.NewGuid().ToString()[..5];

        public FlashSaleOrderConsumer(
            IServiceScopeFactory scopeFactory,
            RabbitMqConnection connection,
            ILogger<FlashSaleOrderConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _connection = connection;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _conn = await _connection.CreateConnectionAsync();
                _channel = await _conn.CreateChannelAsync();

                // Cấu hình prefetch để xử lý từng message một
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        await ProcessMessageAsync(ea, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing flash sale order message. DeliveryTag: {DeliveryTag}", ea.DeliveryTag);

                        // Nack message để retry hoặc đưa vào DLQ
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: "order.created.flashsale.queue",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("FlashSaleOrderConsumer started successfully");

                // Giữ service chạy cho đến khi bị hủy
                Console.WriteLine($"🔥 OrderConsumer started - Instance {InstanceId}");
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("FlashSaleOrderConsumer is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in FlashSaleOrderConsumer");
                throw;
            }
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());

            _logger.LogInformation("Processing flash sale order: {Body}", body);

            var message = JsonSerializer.Deserialize<OrderCreated>(body);

            if (message == null)
            {
                _logger.LogWarning("Failed to deserialize message. DeliveryTag: {DeliveryTag}", ea.DeliveryTag);
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                return;
            }
            Console.WriteLine(
            $"📦 Instance {InstanceId} xử lý message: {message}"
             );
            var flashSaleItems = message.Items.Where(i => i.IsFlashSale).ToList();

            if (!flashSaleItems.Any())
            {
                _logger.LogInformation("No flash sale items in order {OrderId}", message.OrderId);
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                return;
            }
            
            // Xử lý từng flash sale item
            foreach (var item in flashSaleItems)
            {
                try
                {
                    // Lấy flashSaleId hiện tại cho sản phẩm
                    var flashSaleId = !string.IsNullOrEmpty(item.FlashSaleId)
                    ? item.FlashSaleId
                    : await cache.GetActiveFlashSaleIdAsync(item.ProductId);

                    if (string.IsNullOrEmpty(flashSaleId))
                    {
                        _logger.LogWarning(
                            "No active flash sale found for ProductId: {ProductId}, skipping...",
                            item.ProductId
                        );
                        continue;
                    }

                    // Check user đã mua chưa
                    bool alreadyBought = await cache.HasUserBoughtFlashSaleAsync(flashSaleId, item.ProductId, message.UserId);
                    if (alreadyBought)
                    {
                        _logger.LogWarning(
                            "User {UserId} already bought ProductId {ProductId} in FlashSale {FlashSaleId}, skipping...",
                            message.UserId, item.ProductId, flashSaleId
                        );
                        continue; // Skip item nếu user đã mua
                    }


                    bool reserved = await cache.ReserveFlashSaleStockAsync(flashSaleId, item.ProductId);
                    if (!reserved)
                    {
                        _logger.LogWarning(
                            "Not enough flash sale stock for ProductId {ProductId}",
                            item.ProductId);
                        continue;
                    }
                    // 4️⃣ Set TTL cho giữ chỗ (vd: 15 phút)
                    await cache.SetReservationTTLAsync(flashSaleId,item.ProductId,message.OrderId,item.Quantity,TimeSpan.FromMinutes(15));

                    _logger.LogInformation(
                        "Reserved flash sale stock | OrderId: {OrderId}, ProductId: {ProductId}, Qty: {Qty}",
                        message.OrderId, item.ProductId, item.Quantity);

                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to process flash sale item - ProductId: {ProductId}, OrderId: {OrderId}",
                        item.ProductId, message.OrderId);
                    throw; // Re-throw để trigger nack
                }
            }

            // Ack message sau khi xử lý thành công
            await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);

            _logger.LogInformation("Successfully processed flash sale order {OrderId}", message.OrderId);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping FlashSaleOrderConsumer");

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

        public override void Dispose()
        {
            _channel?.Dispose();
            _conn?.Dispose();
            base.Dispose();
        }
    }
}
