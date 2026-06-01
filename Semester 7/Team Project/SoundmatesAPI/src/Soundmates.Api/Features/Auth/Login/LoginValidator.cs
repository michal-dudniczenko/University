using FluentValidation;
using Soundmates.Api.Common.Constants;

namespace Soundmates.Api.Features.Auth.Login;

internal sealed class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaximumUserEmailLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
