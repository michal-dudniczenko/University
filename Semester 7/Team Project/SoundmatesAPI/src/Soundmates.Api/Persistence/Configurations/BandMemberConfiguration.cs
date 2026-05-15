using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class BandMemberConfiguration : IEntityTypeConfiguration<BandMember>
{
    public void Configure(EntityTypeBuilder<BandMember> entity)
    {
        entity.Property(bm => bm.Name)
            .HasMaxLength(50);

        entity
            .HasOne(bm => bm.BandRole)
            .WithMany()
            .HasForeignKey(bm => bm.BandRoleId);
    }
}
