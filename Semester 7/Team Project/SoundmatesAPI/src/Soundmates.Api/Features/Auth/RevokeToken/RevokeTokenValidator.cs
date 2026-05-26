using FluentValidation;

namespace Soundmates.Api.Features.Auth.RevokeToken;

internal sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
