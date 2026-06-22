using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class QuestionChoiceConfiguration : IEntityTypeConfiguration<QuestionChoice>
    {
        public void Configure(EntityTypeBuilder<QuestionChoice> builder)
        {
            builder.HasKey(e => e.QuestionChoiceID);

            builder.Property(e => e.Text)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.OrderIndex)
                .IsRequired();

            builder.HasOne(e => e.SampleQuestion)
                .WithMany(q => q.Choices)
                .HasForeignKey(e => e.SampleQuestionID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.SampleQuestionID);
        }
    }
}