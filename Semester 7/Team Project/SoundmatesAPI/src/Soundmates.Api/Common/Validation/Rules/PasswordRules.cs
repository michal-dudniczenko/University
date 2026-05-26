using FluentValidation;
using Soundmates.Api.Common.Constants;
using System.Buffers;

namespace Soundmates.Api.Common.Validation.Rules;

internal static class PasswordRules
{
    private static readonly SearchValues<char> SpecialChars =
        SearchValues.Create("""!"#$%&'()*+,-./:;<=>?@[\]^_`{|}~""");

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(SecurityConstants.MinimumPasswordLength)
                .WithMessage($"Password is too short. Minimal password length is {SecurityConstants.MinimumPasswordLength}.")
            .MaximumLength(SecurityConstants.MaximumPasswordLength)
                .WithMessage($"Password is too long. Maximum password length is {SecurityConstants.MaximumPasswordLength}.")
            .Must(password => password.All(ch => ch >= 33 && ch <= 126))
                .WithMessage("Password contains invalid characters. Only ASCII printable characters (33-126) are allowed.")
            .Must(password => password.Any(char.IsLower))
                .WithMessage("Password must have at least one lowercase letter.")
            .Must(password => password.Any(char.IsUpper))
                .WithMessage("Password must have at least one uppercase letter.")
            .Must(password => password.Any(char.IsDigit))
                .WithMessage("Password must have at least one digit.")
            .Must(password => password.AsSpan().IndexOfAny(SpecialChars) >= 0)
                .WithMessage("Password must have at least one special character.");
    }
}
