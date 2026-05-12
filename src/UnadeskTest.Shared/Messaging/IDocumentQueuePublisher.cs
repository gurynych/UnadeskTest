namespace UnadeskTest.Shared.Messaging
{
    public interface IDocumentQueuePublisher
    {
        Task PublishAsync(Guid documentId, Guid messageId, CancellationToken cancellationToken);
    }
}
