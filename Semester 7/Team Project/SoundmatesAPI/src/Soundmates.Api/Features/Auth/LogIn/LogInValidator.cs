using FluentValidation;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Features.Auth.LogIn;

internal sealed class LogInValidator : AbstractValidator<LogInRequest>
{
    public LogInValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(MaxUserEmailLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
