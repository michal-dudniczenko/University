using FluentValidation;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Common.Validation.Rules;

internal static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(MinPasswordLength)
                .WithMessage($"Password is too short. Minimal password length is {MinPasswordLength}.")
            .MaximumLength(MaxPasswordLength)
                .WithMessage($"Password is too long. Maximum password length is {MaxPasswordLength}.")
            .Must(password =>
            {
                foreach (char ch in password)
                {
                    if (ch < 33 || ch > 126) return false;
                }
                return true;
            })
                .WithMessage("Password contains invalid characters. Only ASCII printable characters (33-126) are allowed.")
            .Must(password => password.Any(char.IsLower))
                .WithMessage("Password must have at least one lowercase letter.")
            .Must(password => password.Any(char.IsUpper))
                .WithMessage("Password must have at least one uppercase letter.")
            .Must(password => password.Any(char.IsDigit))
                .WithMessage("Password must have at least one digit.")
            .Must(password => password.Any(ch =>
                (ch >= 33 && ch <= 47) || (ch >= 58 && ch <= 64) ||
                (ch >= 91 && ch <= 96) || (ch >= 123 && ch <= 126)))
                .WithMessage("Password must have at least one special character.");
    }
}
