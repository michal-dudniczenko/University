using FluentValidation;
using System.Globalization;
using static Soundmates.Api.Common.Constants.ApplicationConstants;

namespace Soundmates.Api.Common.Validation.Rules;

internal static class BirthDateRules
{
    public static IRuleBuilderOptions<T, string> BirthDate<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(s =>
            {
                if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return false;
                return date >= MinimumUserBirthDate && date <= DateOnly.FromDateTime(DateTime.UtcNow);
            })
            .WithMessage($"Birth date must be in yyyy-MM-dd format and between {MinimumUserBirthDate:yyyy-MM-dd} and today.");
    }
}
