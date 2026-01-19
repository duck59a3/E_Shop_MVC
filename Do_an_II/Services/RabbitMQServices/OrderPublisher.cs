using Do_an_II.Messagings.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Do_an_II.Services.RabbitMQServices
{
    public class OrderPublisher
    {
        private readonly RabbitMqConnection _connection;
        public OrderPublisher(RabbitMqConnection connection)
        {
            _connection = connection;
        }
        // ===============================
        // 1️⃣ ORDER CREATED
        // ===============================
        public async Task PublishOrderCreatedAsync(OrderCreated message)
        {
            await PublishAsync(
                routingKey: "order.created.flashsale",
                message: message
            );
        }

        // ===============================
        // 2️⃣ ORDER CONFIRMED
        // ===============================
        public async Task PublishOrderConfirmedAsync(OrderConfirmed message)
        {
            await PublishAsync(
                routingKey: "order.confirmed.flashsale",
                message: message
            );
        }

        // ===============================
        // CORE PUBLISH METHOD
        // ===============================
        private async Task PublishAsync<T>(string routingKey, T message)
        {
            var connection = await _connection.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "order.exchange",
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body
            );

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
