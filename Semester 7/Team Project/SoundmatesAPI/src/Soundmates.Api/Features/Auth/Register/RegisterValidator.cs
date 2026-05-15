using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Features.Auth.Register;

internal sealed class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(MaxUserEmailLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Password();
    }
}
