using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.DTOs
{
    public record DocumentDto(
        Guid Id,
        string FileName,
        long SizeInBytes,
        DocumentStatus Status,
        string? ErrorMessage,
        DateTime CreatedAtUtc,
        DateTime? ProcessedAtUtc);
}
