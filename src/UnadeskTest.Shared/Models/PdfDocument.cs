namespace UnadeskTest.Shared.Models
{
    public class PdfDocument
    {
        public Guid Id { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public long SizeInBytes { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Queued;

        public string? ExtractedText { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? ProcessingStartedAtUtc { get; set; }

        public DateTime? ProcessedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}
