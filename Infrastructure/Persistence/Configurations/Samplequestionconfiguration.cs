using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SampleQuestionConfiguration : IEntityTypeConfiguration<SampleQuestion>
    {
        public void Configure(EntityTypeBuilder<SampleQuestion> builder)
        {
            builder.HasKey(e => e.SampleQuestionID);

            builder.Property(e => e.Question)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.Difficulty)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.AnswerIndex)
                .IsRequired(false);

            builder.HasOne(e => e.DocumentAnalysis)
                .WithMany(a => a.SampleQuestions)
                .HasForeignKey(e => e.DocumentAnalysisID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.DocumentAnalysisID);
        }
    }
}