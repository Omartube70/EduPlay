using Application.Interfaces.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using UglyToad.PdfPig;

namespace Infrastructure.Services
{
    public class TextExtractionService : ITextExtractionService
    {
        public Task<string> ExtractTextAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => Task.FromResult(ExtractFromPdf(filePath)),
                ".docx" => Task.FromResult(ExtractFromWord(filePath)),
                ".txt" => File.ReadAllTextAsync(filePath),
                _ => throw new NotSupportedException($"Text extraction is not supported for '{extension}' files.")
            };
        }

        private static string ExtractFromPdf(string filePath)
        {
            var sb = new StringBuilder();
            using var document = PdfDocument.Open(filePath);

            foreach (var page in document.GetPages())
                sb.AppendLine(page.Text);

            return sb.ToString();
        }

        private static string ExtractFromWord(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            return body?.InnerText ?? string.Empty;
        }
    }
}