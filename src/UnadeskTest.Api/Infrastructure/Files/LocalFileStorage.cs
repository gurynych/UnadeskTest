using Microsoft.Extensions.Options;
using UnadeskTest.Api.Options;

namespace UnadeskTest.Api.Infrastructure.Files
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly FileStorageOptions options;
        private readonly IWebHostEnvironment environment;

        public LocalFileStorage(IOptions<FileStorageOptions> options, IWebHostEnvironment environment)
        {
            this.options = options.Value;
            this.environment = environment;
        }

        public async Task<StoredFileInfo> SaveAsync(IFormFile file, CancellationToken cancellationToken)
        {
            string rootPath = GetRootPath();
            Directory.CreateDirectory(rootPath);

            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid():N}{extension}";
            string filePath = Path.Combine(rootPath, storedFileName);

            await using FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream, cancellationToken);

            return new StoredFileInfo(Path.GetFileName(file.FileName), storedFileName, filePath, file.Length, file.ContentType);
        }

        private string GetRootPath()
        {
            return Path.IsPathRooted(options.RootPath)
                ? options.RootPath
                : Path.Combine(environment.ContentRootPath, options.RootPath);
        }
    }
}
