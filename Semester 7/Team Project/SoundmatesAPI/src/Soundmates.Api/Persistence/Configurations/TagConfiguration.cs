using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> entity)
    {
        entity
            .HasOne(t => t.TagCategory)
            .WithMany(tc => tc.Tags)
            .HasForeignKey(t => t.TagCategoryId);
    }
}
