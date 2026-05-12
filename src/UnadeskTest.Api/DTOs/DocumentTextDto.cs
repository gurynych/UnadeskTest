using UnadeskTest.Shared.Models;

namespace UnadeskTest.Api.DTOs
{
    public record DocumentTextDto(Guid Id, string FileName, DocumentStatus Status, string Text);
}
