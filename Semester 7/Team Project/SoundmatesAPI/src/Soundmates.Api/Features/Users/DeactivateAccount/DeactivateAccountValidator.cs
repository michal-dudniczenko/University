using FluentValidation;

namespace Soundmates.Api.Features.Users.DeactivateAccount;

internal sealed class DeactivateAccountValidator : AbstractValidator<DeactivateAccountRequest>
{
    public DeactivateAccountValidator()
    {
        RuleFor(x => x.Password).NotEmpty();
    }
}
