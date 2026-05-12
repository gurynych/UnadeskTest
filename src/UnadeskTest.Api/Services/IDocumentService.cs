using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.Services
{
    public interface IDocumentService
    {
        Task<PdfDocument> UploadAsync(IFormFile file, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PdfDocument>> GetAllAsync(CancellationToken cancellationToken);

        Task<PdfDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
