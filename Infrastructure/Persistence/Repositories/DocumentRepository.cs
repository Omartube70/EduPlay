using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
            => await _context.Documents.FirstOrDefaultAsync(d => d.DocumentID == documentId);

        public async Task<Document?> GetDocumentWithAnalysisAsync(int documentId)
            => await _context.Documents
                .Include(d => d.DocumentAnalysis)
                .FirstOrDefaultAsync(d => d.DocumentID == documentId);

        public async Task<IReadOnlyList<Document>> GetDocumentsByUserIdAsync(int userId)
            => await _context.Documents
                .Where(d => d.UserId == userId)
                .Include(d => d.DocumentAnalysis)
                .OrderByDescending(d => d.UploadedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IReadOnlyList<Document>> GetAllDocumentsAsync()
            => await _context.Documents
                .Include(d => d.DocumentAnalysis)
                .OrderByDescending(d => d.UploadedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(Document document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}