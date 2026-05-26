using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class BandRoleConfiguration : IEntityTypeConfiguration<BandRole>
{
    public void Configure(EntityTypeBuilder<BandRole> entity)
    {
        entity.Property(br => br.Name).HasMaxLength(100);
    }
}
