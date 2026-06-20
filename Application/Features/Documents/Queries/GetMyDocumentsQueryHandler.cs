using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetMyDocumentsQueryHandler : IRequestHandler<GetMyDocumentsQuery, IReadOnlyList<DocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public GetMyDocumentsQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<DocumentDto>> Handle(GetMyDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = await _documentRepository.GetDocumentsByUserIdAsync(request.CurrentUserId);
            return _mapper.Map<IReadOnlyList<DocumentDto>>(documents);
        }
    }
}