using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IKeyConceptRepository
    {
        Task<IReadOnlyList<KeyConcept>> GetByAnalysisIdAsync(int analysisId);
        Task<KeyConcept?> GetByIdAsync(int keyConceptId);
        Task AddAsync(KeyConcept keyConcept);
        Task UpdateAsync(KeyConcept keyConcept);
        Task DeleteAsync(KeyConcept keyConcept);
        Task DeleteByAnalysisIdAsync(int analysisId);
    }
}