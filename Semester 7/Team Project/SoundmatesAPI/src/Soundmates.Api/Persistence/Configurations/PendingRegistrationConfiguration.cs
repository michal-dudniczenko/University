using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class PendingRegistrationConfiguration : IEntityTypeConfiguration<PendingRegistration>
{
    public void Configure(EntityTypeBuilder<PendingRegistration> entity)
    {
        entity.Property(pr => pr.Email).HasMaxLength(100);

        entity.Property(pr => pr.PasswordHash).HasMaxLength(256);

        entity.Property(pr => pr.EmailTokenHash)
            .HasMaxLength(32)
            .IsFixedLength();

        entity.HasIndex(pr => pr.EmailTokenHash).IsUnique();
    }
}
