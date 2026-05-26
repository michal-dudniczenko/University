using FluentValidation;
using static Soundmates.Api.Common.Constants.ApplicationConstants;

namespace Soundmates.Api.Features.Auth.Login;

internal sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(MaximumUserEmailLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
