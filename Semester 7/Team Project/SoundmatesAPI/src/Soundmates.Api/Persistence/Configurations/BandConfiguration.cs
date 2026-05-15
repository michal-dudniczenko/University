using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class BandConfiguration : IEntityTypeConfiguration<Band>
{
    public void Configure(EntityTypeBuilder<Band> entity)
    {
        entity
            .HasOne(a => a.User)
            .WithOne();

        entity
            .HasMany(b => b.Members)
            .WithOne(bm => bm.Band)
            .HasForeignKey(bm => bm.BandId);

        entity
            .HasIndex(b => b.UserId)
            .IsUnique();
    }
}
