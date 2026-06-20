using System.Reflection.Emit;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentAnalysis> DocumentAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UserPermissions).IsRequired();
                entity.Property(e => e.UserPermissions).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(e => e.RefreshToken).IsRequired(false);
                entity.Property(e => e.RefreshTokenExpiryTime).IsRequired(false);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentID);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.FileSizeInBytes).IsRequired();
                entity.Property(e => e.ContentType).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.ProcessingStatus).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.UploadedAt).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(u => u.Documents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DocumentAnalysis>(entity =>
            {
                entity.HasKey(e => e.DocumentAnalysisID);
                entity.Property(e => e.ExtractedText).IsRequired();
                entity.Property(e => e.AiSummary).IsRequired();
                entity.Property(e => e.AiResponseJson).IsRequired(false);
                entity.Property(e => e.AnalyzedAt).IsRequired();

                entity.HasOne(a => a.Document)
                    .WithOne(d => d.DocumentAnalysis!)
                    .HasForeignKey<DocumentAnalysis>(a => a.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(a => a.DocumentId).IsUnique();
            });
        }
    }
}