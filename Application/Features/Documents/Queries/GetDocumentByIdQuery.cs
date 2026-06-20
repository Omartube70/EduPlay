using Application.Features.Documents.DTOs;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetDocumentByIdQuery : IRequest<DocumentDto>
    {
        public int DocumentId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}