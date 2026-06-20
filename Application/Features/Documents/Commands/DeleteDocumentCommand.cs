using MediatR;

namespace Application.Features.Documents.Commands
{
    public class DeleteDocumentCommand : IRequest
    {
        public int DocumentId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}