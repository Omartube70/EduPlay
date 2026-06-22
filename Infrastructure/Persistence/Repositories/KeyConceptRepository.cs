using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class KeyConceptRepository : IKeyConceptRepository
    {
        private readonly ApplicationDbContext _context;

        public KeyConceptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<KeyConcept>> GetByAnalysisIdAsync(int analysisId)
            => await _context.KeyConcepts
                .Where(k => k.DocumentAnalysisID == analysisId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<KeyConcept?> GetByIdAsync(int keyConceptId)
            => await _context.KeyConcepts.FindAsync(keyConceptId);

        public async Task AddAsync(KeyConcept keyConcept)
        {
            await _context.KeyConcepts.AddAsync(keyConcept);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KeyConcept keyConcept)
        {
            _context.KeyConcepts.Update(keyConcept);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KeyConcept keyConcept)
        {
            _context.KeyConcepts.Remove(keyConcept);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByAnalysisIdAsync(int analysisId)
        {
            var concepts = await _context.KeyConcepts
                .Where(k => k.DocumentAnalysisID == analysisId)
                .ToListAsync();

            _context.KeyConcepts.RemoveRange(concepts);
            await _context.SaveChangesAsync();
        }
    }
}