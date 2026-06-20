using Application.Features.Documents.DTOs;
using MediatR;

namespace Application.Features.Documents.Commands
{
    /// <summary>
    /// Triggers text extraction + AI summarization for a previously uploaded document.
    /// Can be called directly (sync) or by a background job (DocumentProcessingJob).
    /// </summary>
    public class AnalyzeDocumentCommand : IRequest<DocumentDto>
    {
        public int DocumentId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}