using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DocumentAnalysisRepository : IDocumentAnalysisRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentAnalysisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentAnalysis?> GetByDocumentIdAsync(int documentId)
            => await _context.DocumentAnalyses.FirstOrDefaultAsync(a => a.DocumentId == documentId);

        public async Task AddAsync(DocumentAnalysis analysis)
        {
            await _context.DocumentAnalyses.AddAsync(analysis);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentAnalysis analysis)
        {
            _context.DocumentAnalyses.Update(analysis);
            await _context.SaveChangesAsync();
        }
    }
}