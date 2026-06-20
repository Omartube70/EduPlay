using Application.Features.Documents.DTOs;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetMyDocumentsQuery : IRequest<IReadOnlyList<DocumentDto>>
    {
        public int CurrentUserId { get; set; }
    }
}