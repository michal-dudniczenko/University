using FluentValidation;
using Soundmates.Api.Common.Constants;

namespace Soundmates.Api.Features.Auth.ForgotPassword;

internal sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaximumUserEmailLength)
            .EmailAddress();
    }
}
