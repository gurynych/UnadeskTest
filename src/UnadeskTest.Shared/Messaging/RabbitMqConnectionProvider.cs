using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UnadeskTest.Shared.Options;

namespace UnadeskTest.Shared.Messaging
{
    public class RabbitMqConnectionProvider : IRabbitMqConnectionProvider
    {
        private readonly RabbitMqOptions options;
        private readonly object syncRoot = new object();
        private IConnection? connection;

        public RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options)
        {
            this.options = options.Value;
        }

        public IConnection GetConnection()
        {
            if (connection is { IsOpen: true })
            {
                return connection;
            }

            lock (syncRoot)
            {
                if (connection is { IsOpen: true })
                {
                    return connection;
                }

                ConnectionFactory factory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    UserName = options.UserName,
                    Password = options.Password,
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true
                };

                connection = factory.CreateConnection();
                return connection;
            }
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}
