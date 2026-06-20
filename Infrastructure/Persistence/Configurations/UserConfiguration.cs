using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.UserID);

            builder.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.UserName).IsUnique();

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.UserPermissions)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.RefreshToken).IsRequired(false);
            builder.Property(e => e.RefreshTokenExpiryTime).IsRequired(false);

            builder.HasMany(u => u.Documents)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}