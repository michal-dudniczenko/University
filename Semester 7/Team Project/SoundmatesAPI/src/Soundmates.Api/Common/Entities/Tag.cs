namespace Soundmates.Api.Common.Entities;

internal sealed class Tag : EntityBase
{
    public required string Name { get; set; }

    public required Guid TagCategoryId { get; set; }
    public TagCategory TagCategory { get; set; } = null!;
}
