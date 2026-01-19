using RabbitMQ.Client;
using System.Threading.Tasks;

namespace Do_an_II.Services.RabbitMQServices
{
    public class RabbitMqConnection
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqConnection(IConfiguration config)
        {
            _factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:HostName"],
                UserName = config["RabbitMQ:UserName"],
                Password = config["RabbitMQ:Password"],
                VirtualHost = config["RabbitMQ:VirtualHost"]
                // Removed DispatchConsumersAsync = true; as it does not exist on ConnectionFactory
            };
        }

        public async Task<IConnection> CreateConnectionAsync()
            => await _factory.CreateConnectionAsync();
    }
}
