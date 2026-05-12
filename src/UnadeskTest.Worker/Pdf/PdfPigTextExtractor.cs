using System.Text;

namespace UnadeskTest.Worker.Pdf
{
    public class PdfPigTextExtractor : IPdfTextExtractor
    {
        public Task<string> ExtractAsync(string filePath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            StringBuilder text = new StringBuilder();

            using UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(filePath);
            foreach (UglyToad.PdfPig.Content.Page page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();
                text.AppendLine(page.Text);
            }

            return Task.FromResult(text.ToString());
        }
    }
}
