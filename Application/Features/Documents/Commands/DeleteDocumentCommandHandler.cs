using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Documents.Commands
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IFileStorageService _fileStorageService;

        public DeleteDocumentCommandHandler(IDocumentRepository documentRepository, IFileStorageService fileStorageService)
        {
            _documentRepository = documentRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(request.DocumentId);

            if (document == null)
                throw new DocumentNotFoundException(request.DocumentId);

            if (!request.IsAdmin && document.UserId != request.CurrentUserId)
                throw new ForbiddenAccessException("You don't have permission to delete this document.");

            await _fileStorageService.DeleteFileAsync(document.FilePath);
            await _documentRepository.DeleteDocumentAsync(document);
        }
    }
}