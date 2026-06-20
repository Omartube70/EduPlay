using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document?> GetDocumentByIdAsync(int documentId);
        Task<Document?> GetDocumentWithAnalysisAsync(int documentId);
        Task<IReadOnlyList<Document>> GetDocumentsByUserIdAsync(int userId);
        Task<IReadOnlyList<Document>> GetAllDocumentsAsync();
        Task AddDocumentAsync(Document document);
        Task UpdateDocumentAsync(Document document);
        Task DeleteDocumentAsync(Document document);
    }
}