using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(e => e.DocumentID);

            builder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.FilePath).IsRequired();
            builder.Property(e => e.FileSizeInBytes).IsRequired();

            builder.Property(e => e.ContentType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.ProcessingStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.UploadedAt).IsRequired();

            builder.HasIndex(e => e.UserId);
        }
    }
}