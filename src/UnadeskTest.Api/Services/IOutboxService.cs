namespace UnadeskTest.Api.Services
{
    public interface IOutboxService
    {
        Task PublishPendingAsync(CancellationToken cancellationToken);
    }
}
