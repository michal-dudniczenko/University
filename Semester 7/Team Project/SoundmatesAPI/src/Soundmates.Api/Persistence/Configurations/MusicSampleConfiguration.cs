using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class MusicSampleConfiguration : IEntityTypeConfiguration<MusicSample>
{
    public void Configure(EntityTypeBuilder<MusicSample> entity)
    {
        entity.Property(ms => ms.FileName).HasMaxLength(100);
    }
}
