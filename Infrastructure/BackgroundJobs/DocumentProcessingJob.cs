using Application.Features.Documents.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Picks up documents that are still in "Pending" status and runs analysis (text extraction + AI summary).
    /// Can be triggered manually via AnalyzeDocumentCommand, or wired into a queue/hosted service later.
    /// </summary>
    public class DocumentProcessingJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DocumentProcessingJob> _logger;

        public DocumentProcessingJob(IServiceScopeFactory scopeFactory, ILogger<DocumentProcessingJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ProcessAsync(int documentId, int ownerUserId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                await mediator.Send(new AnalyzeDocumentCommand
                {
                    DocumentId = documentId,
                    CurrentUserId = ownerUserId,
                    IsAdmin = true
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process document {DocumentId}", documentId);
            }
        }
    }
}