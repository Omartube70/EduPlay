using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ISampleQuestionRepository
    {
        Task<IReadOnlyList<SampleQuestion>> GetByAnalysisIdAsync(int analysisId);
        Task<SampleQuestion?> GetByIdWithChoicesAsync(int questionId);
        Task AddAsync(SampleQuestion question);
        Task AddChoicesAsync(int questionId, List<string> choices);
        Task UpdateAsync(SampleQuestion question);
        Task DeleteAsync(SampleQuestion question);
        Task DeleteByAnalysisIdAsync(int analysisId);
    }
}