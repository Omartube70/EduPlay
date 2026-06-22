using Application.Exceptions;
using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Documents.Commands
{
    public class AnalyzeDocumentCommandHandler : IRequestHandler<AnalyzeDocumentCommand, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentAnalysisRepository _analysisRepository;
        private readonly ITextExtractionService _textExtractionService;
        private readonly IAIService _aiService;
        private readonly IMapper _mapper;

        public AnalyzeDocumentCommandHandler(
            IDocumentRepository documentRepository,
            IDocumentAnalysisRepository analysisRepository,
            ITextExtractionService textExtractionService,
            IAIService aiService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _analysisRepository = analysisRepository;
            _textExtractionService = textExtractionService;
            _aiService = aiService;
            _mapper = mapper;
        }

        public async Task<DocumentDto> Handle(AnalyzeDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetDocumentWithAnalysisAsync(request.DocumentId);

            if (document == null)
                throw new DocumentNotFoundException(request.DocumentId);

           

            if (!request.IsAdmin && document.UserId != request.CurrentUserId)
                throw new ForbiddenAccessException("You don't have permission to analyze this document.");


            document.MarkAsProcessing();
            await _documentRepository.UpdateDocumentAsync(document);

            try
            {
                var extractedText = await _textExtractionService.ExtractTextAsync(document.FilePath);
                var (summary, responseJson) = await _aiService.SummarizeAsync(extractedText, cancellationToken);

                var existingAnalysis = await _analysisRepository.GetByDocumentIdAsync(document.DocumentID);

                if (existingAnalysis == null)
                {
                    var analysis = DocumentAnalysis.Create(extractedText, summary, responseJson, document.DocumentID);
                    await _analysisRepository.AddAsync(analysis);
                }
                else
                {
                    existingAnalysis.UpdateSummary(summary, responseJson);
                    await _analysisRepository.UpdateAsync(existingAnalysis);
                }

                document.MarkAsCompleted();
            }
            catch
            {
                document.MarkAsFailed();
                await _documentRepository.UpdateDocumentAsync(document);
                throw;
            }

            await _documentRepository.UpdateDocumentAsync(document);

            var updatedDocument = await _documentRepository.GetDocumentWithAnalysisAsync(document.DocumentID);
            return _mapper.Map<DocumentDto>(updatedDocument);
        }
    }
}