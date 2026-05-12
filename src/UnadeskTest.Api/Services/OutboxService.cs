using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnadeskTest.Api.Options;
using UnadeskTest.Shared.Data;
using UnadeskTest.Shared.Messaging;
using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly AppDbContext context;
        private readonly IDocumentQueuePublisher queuePublisher;
        private readonly OutboxOptions options;
        private readonly ILogger<OutboxService> logger;

        public OutboxService(AppDbContext context, IDocumentQueuePublisher queuePublisher, IOptions<OutboxOptions> options, ILogger<OutboxService> logger)
        {
            this.context = context;
            this.queuePublisher = queuePublisher;
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task PublishPendingAsync(CancellationToken cancellationToken)
        {
            OutboxMessage[] messages = await context.OutboxMessages
                .Where(m => m.Status == OutboxMessageStatus.Pending)
                .OrderBy(m => m.CreatedAtUtc)
                .Take(options.BatchSize)
                .ToArrayAsync(cancellationToken);

            if (messages.Length == 0)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;

            foreach (OutboxMessage message in messages)
            {
                try
                {
                    await queuePublisher.PublishAsync(message.DocumentId, message.Id, cancellationToken);

                    message.Status = OutboxMessageStatus.Published;
                    message.PublishedAtUtc = now;
                    message.UpdatedAtUtc = now;
                    message.ErrorMessage = null;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Не удалось опубликовать Outbox сообщение {MessageId}.", message.Id);

                    message.PublishAttempts++;
                    message.ErrorMessage = exception.Message;
                    message.UpdatedAtUtc = now;

                    if (message.PublishAttempts >= options.MaxPublishAttempts)
                    {
                        message.Status = OutboxMessageStatus.Failed;
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
