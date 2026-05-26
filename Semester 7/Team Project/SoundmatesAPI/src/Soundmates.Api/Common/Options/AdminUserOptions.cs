using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.Common.Options;

internal sealed class AdminUserOptions
{
    public const string SectionName = "AdminUser";
    public const string AdminRoleName = "Admin";

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
