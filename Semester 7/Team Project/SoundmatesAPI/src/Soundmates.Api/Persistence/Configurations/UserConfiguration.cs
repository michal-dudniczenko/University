using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.Property(u => u.Name)
            .HasMaxLength(50);

        entity.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(u => u.ProfileDescription)
            .HasMaxLength(500);

        entity
            .HasOne(u => u.Country)
            .WithMany()
            .HasForeignKey(u => u.CountryId);

        entity
            .HasOne(u => u.City)
            .WithMany()
            .HasForeignKey(u => u.CityId);

        entity
            .HasMany(u => u.Tags)
            .WithMany();

        entity
            .HasMany(u => u.ProfilePictures)
            .WithOne(pp => pp.User)
            .HasForeignKey(pp => pp.UserId);

        entity
            .HasMany(u => u.MusicSamples)
            .WithOne(ms => ms.User)
            .HasForeignKey(ms => ms.UserId);

        entity
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId);
    }
}
