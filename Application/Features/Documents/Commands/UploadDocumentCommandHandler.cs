using Application.Features.Documents.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Documents.Commands
{
    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, DocumentDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public UploadDocumentCommandHandler(
            IDocumentRepository documentRepository,
            IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task<DocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var extension = System.IO.Path.GetExtension(request.File.FileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".pdf" => ContentType.Pdf,
                ".doc" or ".docx" => ContentType.Word,
                ".txt" => ContentType.Txt,
                _ => throw new ArgumentException("Unsupported file type.")
            };

            var (filePath, sizeInBytes) = await _fileStorageService.SaveFileAsync(request.File);

            var document = Document.Create(
                request.File.FileName,
                filePath,
                sizeInBytes,
                contentType,
                request.CurrentUserId);

            await _documentRepository.AddDocumentAsync(document);

            return _mapper.Map<DocumentDto>(document);
        }
    }
}