using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class UserMatchPreferenceConfiguration : IEntityTypeConfiguration<UserMatchPreference>
{
    public void Configure(EntityTypeBuilder<UserMatchPreference> entity)
    {
        entity
            .HasOne(ump => ump.User)
            .WithOne()
            .HasForeignKey<UserMatchPreference>(ump => ump.UserId);

        entity
            .HasMany(ump => ump.Tags)
            .WithMany();

        entity
            .HasIndex(ump => ump.UserId)
            .IsUnique();
    }
}
