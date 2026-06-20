using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DocumentAnalysisConfiguration : IEntityTypeConfiguration<DocumentAnalysis>
    {
        public void Configure(EntityTypeBuilder<DocumentAnalysis> builder)
        {
            builder.HasKey(e => e.DocumentAnalysisID);

            builder.Property(e => e.ExtractedText).IsRequired();
            builder.Property(e => e.AiSummary).IsRequired();
            builder.Property(e => e.AiResponseJson).IsRequired(false);
            builder.Property(e => e.AnalyzedAt).IsRequired();

            builder.HasOne(a => a.Document)
                .WithOne(d => d.DocumentAnalysis!)
                .HasForeignKey<DocumentAnalysis>(a => a.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.DocumentId).IsUnique();
        }
    }
}