namespace UnadeskTest.Api.Infrastructure.Files
{
    public record StoredFileInfo(string OriginalFileName, string StoredFileName, string FilePath, long SizeInBytes, string ContentType);
}
