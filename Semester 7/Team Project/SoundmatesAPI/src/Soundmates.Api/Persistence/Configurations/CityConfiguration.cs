using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> entity)
    {
        entity.Property(c => c.Name).HasMaxLength(100);

        entity
            .HasOne(c => c.Country)
            .WithMany()
            .HasForeignKey(c => c.CountryId);
    }
}
