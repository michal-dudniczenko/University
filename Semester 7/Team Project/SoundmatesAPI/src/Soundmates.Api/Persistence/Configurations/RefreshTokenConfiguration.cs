using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity
            .HasOne(rt => rt.User)
            .WithOne()
            .HasForeignKey<RefreshToken>(rt => rt.UserId);

        entity
            .HasIndex(rt => rt.UserId)
            .IsUnique();
    }
}
