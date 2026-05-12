using Microsoft.EntityFrameworkCore;
using UnadeskTest.Api.Infrastructure.Files;
using UnadeskTest.Shared.Data;
using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext context;
        private readonly IFileStorage fileStorage;

        public DocumentService(AppDbContext context, IFileStorage fileStorage)
        {
            this.context = context;
            this.fileStorage = fileStorage;
        }

        public async Task<PdfDocument> UploadAsync(IFormFile file, CancellationToken cancellationToken)
        {
            ValidateFile(file);

            StoredFileInfo storedFile = await fileStorage.SaveAsync(file, cancellationToken);
            DateTime now = DateTime.UtcNow;

            PdfDocument document = new PdfDocument
            {
                Id = Guid.NewGuid(),
                OriginalFileName = storedFile.OriginalFileName,
                StoredFileName = storedFile.StoredFileName,
                FilePath = storedFile.FilePath,
                SizeInBytes = storedFile.SizeInBytes,
                ContentType = storedFile.ContentType,
                Status = DocumentStatus.Queued,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            OutboxMessage outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                Status = OutboxMessageStatus.Pending,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            context.PdfDocuments.Add(document);
            context.OutboxMessages.Add(outboxMessage);
            await context.SaveChangesAsync(cancellationToken);

            return document;
        }

        public Task<IReadOnlyCollection<PdfDocument>> GetAllAsync(CancellationToken cancellationToken)
        {
            return GetAllInternalAsync(cancellationToken);
        }

        private async Task<IReadOnlyCollection<PdfDocument>> GetAllInternalAsync(CancellationToken cancellationToken)
        {
            PdfDocument[] documents = await context.PdfDocuments
                .AsNoTracking()
                .OrderByDescending(d => d.CreatedAtUtc)
                .ToArrayAsync(cancellationToken);
            return documents;
        }

        public Task<PdfDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return context.PdfDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException("Загруженный файл пуст.");
            }

            string extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Поддерживаются только файлы в формате PDF.");
            }
        }
    }
}
