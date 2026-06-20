using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IDocumentAnalysisRepository
    {
        Task<DocumentAnalysis?> GetByDocumentIdAsync(int documentId);
        Task AddAsync(DocumentAnalysis analysis);
        Task UpdateAsync(DocumentAnalysis analysis);
    }
}