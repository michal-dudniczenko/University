using FluentValidation;

namespace Soundmates.Api.Features.Auth.Refresh;

internal sealed class RefreshValidator : AbstractValidator<RefreshRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
