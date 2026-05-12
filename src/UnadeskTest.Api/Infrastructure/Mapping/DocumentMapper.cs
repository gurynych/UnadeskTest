using UnadeskTest.Api.DTOs;
using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.Infrastructure.Mapping
{
    public static class DocumentMapper
    {
        public static DocumentDto ToDto(PdfDocument document) =>
            new DocumentDto(document.Id, document.OriginalFileName, document.SizeInBytes, document.Status,
                document.ErrorMessage, document.CreatedAtUtc, document.ProcessedAtUtc);

        public static DocumentTextDto ToTextDto(PdfDocument document) =>
            new DocumentTextDto(document.Id, document.OriginalFileName, document.Status,
                document.ExtractedText ?? string.Empty);
    }
}
