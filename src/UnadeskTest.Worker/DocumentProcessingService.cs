using UnadeskTest.Shared.Data;
using UnadeskTest.Shared.Models;
using UnadeskTest.Worker.Pdf;

namespace UnadeskTest.Worker
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly AppDbContext context;
        private readonly IPdfTextExtractor pdfTextExtractor;
        private readonly ILogger<DocumentProcessingService> logger;

        public DocumentProcessingService(AppDbContext context, IPdfTextExtractor pdfTextExtractor, ILogger<DocumentProcessingService> logger)
        {
            this.context = context;
            this.pdfTextExtractor = pdfTextExtractor;
            this.logger = logger;
        }

        public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken)
        {
            PdfDocument? document = await context.PdfDocuments.FindAsync(new object[] { documentId }, cancellationToken);

            if (document is null)
            {
                logger.LogWarning("Документ {DocumentId} не найден.", documentId);
                return;
            }

            if (document.Status == DocumentStatus.Processed)
            {
                logger.LogInformation("Документ {DocumentId} уже обработан.", documentId);
                return;
            }

            DateTime now = DateTime.UtcNow;
            document.Status = DocumentStatus.Processing;
            document.ProcessingStartedAtUtc = now;
            document.UpdatedAtUtc = now;
            document.ErrorMessage = null;
            await context.SaveChangesAsync(cancellationToken);

            try
            {
                string extractedText = await pdfTextExtractor.ExtractAsync(document.FilePath, cancellationToken);

                now = DateTime.UtcNow;
                document.ExtractedText = extractedText;
                document.Status = DocumentStatus.Processed;
                document.ProcessedAtUtc = now;
                document.UpdatedAtUtc = now;
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Ошибка обработки документа {DocumentId}.", documentId);

                now = DateTime.UtcNow;
                document.Status = DocumentStatus.Failed;
                document.ErrorMessage = exception.Message;
                document.UpdatedAtUtc = now;
                await context.SaveChangesAsync(CancellationToken.None);
                throw;
            }
        }
    }
}
