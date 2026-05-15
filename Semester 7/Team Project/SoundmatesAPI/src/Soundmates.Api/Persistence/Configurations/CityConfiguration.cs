using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> entity)
    {
        entity
            .HasOne(c => c.Country)
            .WithMany()
            .HasForeignKey(c => c.CountryId);
    }
}
