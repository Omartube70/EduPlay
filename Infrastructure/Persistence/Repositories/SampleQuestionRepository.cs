using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SampleQuestionRepository : ISampleQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public SampleQuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SampleQuestion>> GetByAnalysisIdAsync(int analysisId)
            => await _context.SampleQuestions
                .Where(q => q.DocumentAnalysisID == analysisId)
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync();

        public async Task<SampleQuestion?> GetByIdWithChoicesAsync(int questionId)
            => await _context.SampleQuestions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.SampleQuestionID == questionId);

        public async Task AddAsync(SampleQuestion question)
        {
            await _context.SampleQuestions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task AddChoicesAsync(int questionId, List<string> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = QuestionChoice.Create(choices[i], i, questionId);
                await _context.QuestionChoices.AddAsync(choice);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SampleQuestion question)
        {
            _context.SampleQuestions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SampleQuestion question)
        {
            _context.SampleQuestions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByAnalysisIdAsync(int analysisId)
        {
            var questions = await _context.SampleQuestions
                .Where(q => q.DocumentAnalysisID == analysisId)
                .ToListAsync();

            _context.SampleQuestions.RemoveRange(questions);
            await _context.SaveChangesAsync();
        }
    }
}