namespace UnadeskTest.Api.Infrastructure.Files
{
    public interface IFileStorage
    {
        Task<StoredFileInfo> SaveAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
