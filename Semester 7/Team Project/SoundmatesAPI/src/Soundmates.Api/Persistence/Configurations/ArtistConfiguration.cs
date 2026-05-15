using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> entity)
    {
        entity
            .HasOne(a => a.User)
            .WithOne();

        entity
            .HasOne(a => a.Gender)
            .WithMany()
            .HasForeignKey(a => a.GenderId);

        entity
            .HasIndex(a => a.UserId)
            .IsUnique();
    }
}
