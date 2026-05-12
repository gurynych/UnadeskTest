using RabbitMQ.Client;

namespace UnadeskTest.Shared.Messaging
{
    public interface IRabbitMqConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
