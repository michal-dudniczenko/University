using FluentValidation;
using System.Globalization;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Common.Validation.Rules;

internal static class BirthDateRules
{
    public static IRuleBuilderOptions<T, string> BirthDate<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                .WithMessage("Birth date must be in yyyy-MM-dd format.")
            .Must(s =>
                {
                    if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        return true; // already caught above

                    return date >= MinUserBirthDate && date <= DateOnly.FromDateTime(DateTime.UtcNow);
                })
                .WithMessage($"Birth date must be between {MinUserBirthDate:yyyy-MM-dd} and today.");
    }
}
