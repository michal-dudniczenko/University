namespace Soundmates.Api.Common.Entities;

internal sealed class TagCategory : EntityBase
{
    public required string Name { get; set; }
    public required bool IsForBand { get; set; }

    public ICollection<Tag> Tags { get; } = [];
}
