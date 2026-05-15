using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> entity)
    {
        entity
            .HasOne(l => l.Giver)
            .WithMany()
            .HasForeignKey(l => l.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(l => l.Receiver)
            .WithMany()
            .HasForeignKey(l => l.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasIndex(l => new { l.GiverId, l.ReceiverId })
            .IsUnique();
    }
}
