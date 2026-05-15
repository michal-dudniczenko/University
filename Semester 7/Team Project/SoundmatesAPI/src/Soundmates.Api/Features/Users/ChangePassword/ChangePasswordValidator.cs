using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Users.ChangePassword;

internal sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().Password();
    }
}
