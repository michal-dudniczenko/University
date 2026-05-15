using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> entity)
    {
        entity
            .HasOne(m => m.User1)
            .WithMany()
            .HasForeignKey(m => m.User1Id);

        entity
            .HasOne(m => m.User2)
            .WithMany()
            .HasForeignKey(m => m.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
