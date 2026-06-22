using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentAnalysis> DocumentAnalyses => Set<DocumentAnalysis>();
        public DbSet<KeyConcept> KeyConcepts => Set<KeyConcept>();
        public DbSet<SampleQuestion> SampleQuestions => Set<SampleQuestion>();
        public DbSet<QuestionChoice> QuestionChoices => Set<QuestionChoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}