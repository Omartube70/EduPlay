namespace Application.Features.Documents.DTOs
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSizeInBytes { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string ProcessingStatus { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int UserId { get; set; }
        public DocumentAnalysisDto? Analysis { get; set; }
    }
}