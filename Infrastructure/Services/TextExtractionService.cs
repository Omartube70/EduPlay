using Application.Interfaces.Services;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;

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
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                var text = new StringBuilder();

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var strategy = new LocationTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                    text.AppendLine(pageText);
                }

                return text.ToString();
            }
        }

        private static string ExtractFromWord(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            return body?.InnerText ?? string.Empty;
        }
    }
}