using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UnadeskTest.Shared.Options;

namespace UnadeskTest.Shared.Messaging
{
    public class RabbitMqDocumentQueuePublisher : IDocumentQueuePublisher
    {
        private readonly IRabbitMqConnectionProvider connectionProvider;
        private readonly RabbitMqOptions options;

        public RabbitMqDocumentQueuePublisher(IRabbitMqConnectionProvider connectionProvider, IOptions<RabbitMqOptions> options)
        {
            this.connectionProvider = connectionProvider;
            this.options = options.Value;
        }

        public Task PublishAsync(Guid documentId, Guid messageId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using IModel channel = connectionProvider.GetConnection().CreateModel();
            channel.QueueDeclare(options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            DocumentProcessingMessage message = new DocumentProcessingMessage
            {
                MessageId = messageId,
                DocumentId = documentId
            };

            byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = messageId.ToString();

            channel.BasicPublish(exchange: string.Empty, routingKey: options.QueueName, basicProperties: properties, body: body);

            return Task.CompletedTask;
        }
    }
}
