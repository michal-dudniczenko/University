namespace Soundmates.Api.Common.Entities;

internal sealed class Artist : EntityBase
{
    public required DateOnly BirthDate { get; set; }

    public required Guid GenderId { get; set; }
    public Gender Gender { get; set; } = null!;

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
