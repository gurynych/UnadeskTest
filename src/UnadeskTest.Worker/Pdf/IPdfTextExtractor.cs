namespace UnadeskTest.Worker.Pdf
{
    public interface IPdfTextExtractor
    {
        Task<string> ExtractAsync(string filePath, CancellationToken cancellationToken);
    }
}
