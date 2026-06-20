using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Documents.Queries
{
    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IReadOnlyList<DocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;

        public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = await _documentRepository.GetAllDocumentsAsync();
            return _mapper.Map<IReadOnlyList<DocumentDto>>(documents);
        }
    }
}