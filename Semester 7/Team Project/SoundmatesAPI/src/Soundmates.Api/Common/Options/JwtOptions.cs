using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.Common.Options;

internal sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required int ExpirationInMinutes { get; set; }

    [Required, MinLength(32)]
    public required string SecretKey { get; set; }

    [Required]
    public required int RefreshTokenLifetimeInDays { get; set; }
}
