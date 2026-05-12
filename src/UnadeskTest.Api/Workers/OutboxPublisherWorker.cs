using Microsoft.Extensions.Options;
using UnadeskTest.Api.Options;
using UnadeskTest.Api.Services;

namespace UnadeskTest.Api.Workers
{
    public class OutboxPublisherWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly OutboxOptions options;
        private readonly ILogger<OutboxPublisherWorker> logger;

        public OutboxPublisherWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<OutboxOptions> options,
            ILogger<OutboxPublisherWorker> logger)
        {
            this.scopeFactory = scopeFactory;
            this.options = options.Value;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Outbox publisher запущен.");

            TimeSpan interval = TimeSpan.FromSeconds(options.PublishIntervalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = scopeFactory.CreateScope();
                    IOutboxService outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                    await outboxService.PublishPendingAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Ошибка при публикации сообщений из Outbox.");
                }

                try
                {
                    await Task.Delay(interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }
}
