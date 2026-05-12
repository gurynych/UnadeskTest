namespace UnadeskTest.Worker
{
    public interface IDocumentProcessingService
    {
        Task ProcessAsync(Guid documentId, CancellationToken cancellationToken);
    }
}
