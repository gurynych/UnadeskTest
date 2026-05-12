using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UnadeskTest.Shared.Messaging;
using UnadeskTest.Shared.Options;

namespace UnadeskTest.Worker
{
    public class PdfProcessingWorker : BackgroundService
    {
        private readonly IRabbitMqConnectionProvider connectionProvider;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly RabbitMqOptions options;
        private readonly ILogger<PdfProcessingWorker> logger;

        private IModel? channel;

        public PdfProcessingWorker(IRabbitMqConnectionProvider connectionProvider, IServiceScopeFactory scopeFactory, IOptions<RabbitMqOptions> options, ILogger<PdfProcessingWorker> logger)
        {
            this.connectionProvider = connectionProvider;
            this.scopeFactory = scopeFactory;
            this.options = options.Value;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            channel = connectionProvider.GetConnection().CreateModel();
            channel.QueueDeclare(options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: options.PrefetchCount, global: false);

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;

            channel.BasicConsume(queue: options.QueueName, autoAck: false, consumer: consumer);
            logger.LogInformation("Worker запущен. Очередь: {QueueName}", options.QueueName);
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs args)
        {
            if (channel is null)
            {
                return;
            }

            DocumentProcessingMessage? message = TryDeserialize(args.Body.ToArray());
            if (message is null)
            {
                logger.LogWarning("Получено некорректное сообщение. DeliveryTag={DeliveryTag}", args.DeliveryTag);
                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                IDocumentProcessingService processingService = scope.ServiceProvider.GetRequiredService<IDocumentProcessingService>();
                await processingService.ProcessAsync(message.DocumentId, CancellationToken.None);

                channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Не удалось обработать сообщение {MessageId} для документа {DocumentId}.",
                    message.MessageId, message.DocumentId);

                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
            }
        }

        private static DocumentProcessingMessage? TryDeserialize(byte[] body)
        {
            try
            {
                string json = Encoding.UTF8.GetString(body);
                return JsonSerializer.Deserialize<DocumentProcessingMessage>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public override void Dispose()
        {
            channel?.Dispose();
            base.Dispose();
        }
    }
}
