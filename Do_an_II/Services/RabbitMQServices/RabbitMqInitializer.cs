using RabbitMQ.Client;

namespace Do_an_II.Services.RabbitMQServices
{
    public class RabbitMqInitializer
    {
        public static async Task InitAsync(IConnection connection)
        {
            var channel = await connection.CreateChannelAsync();

            
                // 1. Declare Exchange
                await channel.ExchangeDeclareAsync(
                    exchange: "order.exchange",
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false
                );

            // ======================
            // 2️⃣ OrderCreated → Reserve flash sale
            // ======================
            await channel.QueueDeclareAsync(
                queue: "order.created.flashsale.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: "order.created.flashsale.queue",
                exchange: "order.exchange",
                routingKey: "order.created"
            );

            // ======================
            // 3️⃣ OrderConfirmed → Commit stock
            // ======================
            await channel.QueueDeclareAsync(
                queue: "order.confirmed.flashsale.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: "order.confirmed.flashsale.queue",
                exchange: "order.exchange",
                routingKey: "order.confirmed"
            );

            Console.WriteLine("RabbitMQ initialized successfully!");
            
            
                await channel.CloseAsync();
            
        }
    }
}
