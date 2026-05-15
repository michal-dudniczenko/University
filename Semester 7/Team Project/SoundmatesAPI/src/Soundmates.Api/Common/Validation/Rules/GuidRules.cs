using FluentValidation;

namespace Soundmates.Api.Common.Validation.Rules;

internal static class GuidRules
{
    public static IRuleBuilderOptions<T, string> ValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .Must(s => Guid.TryParse(s, out _))
            .WithMessage("{PropertyName} must be a valid GUID.");
}
