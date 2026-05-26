using Microsoft.AspNetCore.Identity;

namespace Soundmates.Api.Common.Entities;

internal sealed class User : IdentityUser<Guid>
{
    public override Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public override required string Email { get; set; }

    public bool? IsBand { get; set; }
    public string? Name { get; set; }
    public string? ProfileDescription { get; set; }

    public bool IsFirstLogin { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime? DeactivatedAt { get; set; }

    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    public Guid? CityId { get; set; }
    public City? City { get; set; }

    public ICollection<Tag> Tags { get; } = [];
    public ICollection<ProfilePicture> ProfilePictures { get; } = [];
    public ICollection<MusicSample> MusicSamples { get; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; } = [];
}
