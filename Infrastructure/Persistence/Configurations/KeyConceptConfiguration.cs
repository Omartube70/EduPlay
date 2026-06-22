using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class KeyConceptConfiguration : IEntityTypeConfiguration<KeyConcept>
    {
        public void Configure(EntityTypeBuilder<KeyConcept> builder)
        {
            builder.HasKey(e => e.KeyConceptID);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .IsRequired();

            builder.HasOne(e => e.DocumentAnalysis)
                .WithMany(a => a.KeyConcepts)
                .HasForeignKey(e => e.DocumentAnalysisID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.DocumentAnalysisID);
        }
    }
}