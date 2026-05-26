using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(32)
            .IsFixedLength();

        entity.HasIndex(rt => rt.TokenHash);
        entity.HasIndex(rt => rt.UserId);
    }
}
