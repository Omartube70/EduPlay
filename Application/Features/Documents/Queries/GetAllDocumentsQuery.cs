using Application.Features.Documents.DTOs;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetAllDocumentsQuery : IRequest<IReadOnlyList<DocumentDto>>
    {
    }
}