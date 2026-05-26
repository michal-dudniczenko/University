using FluentValidation;
using Soundmates.Api.Common.Constants;

namespace Soundmates.Api.Features.Auth.ResendEmailConfirmation;

internal sealed class ResendEmailConfirmationValidator : AbstractValidator<ResendEmailConfirmationRequest>
{
    public ResendEmailConfirmationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaximumUserEmailLength)
            .EmailAddress();
    }
}
