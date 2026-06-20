using Application.Exceptions;
using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public GetDocumentByIdQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetDocumentWithAnalysisAsync(request.DocumentId);

            if (document == null)
                throw new DocumentNotFoundException(request.DocumentId);

            if (!request.IsAdmin && document.UserId != request.CurrentUserId)
                throw new ForbiddenAccessException("You don't have permission to view this document.");

            return _mapper.Map<DocumentDto>(document);
        }
    }
}