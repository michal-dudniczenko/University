using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class DislikeConfiguration : IEntityTypeConfiguration<Dislike>
{
    public void Configure(EntityTypeBuilder<Dislike> entity)
    {
        entity
            .HasOne(dl => dl.Giver)
            .WithMany()
            .HasForeignKey(dl => dl.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(dl => dl.Receiver)
            .WithMany()
            .HasForeignKey(dl => dl.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasIndex(dl => new { dl.GiverId, dl.ReceiverId })
            .IsUnique();
    }
}
