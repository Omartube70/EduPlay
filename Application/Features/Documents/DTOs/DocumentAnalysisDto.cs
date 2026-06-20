namespace Application.Features.Documents.DTOs
{
    public class DocumentAnalysisDto
    {
        public int DocumentAnalysisId { get; set; }
        public string ExtractedText { get; set; } = string.Empty;
        public string AiSummary { get; set; } = string.Empty;
        public string? AiResponseJson { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}