using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Matching.CreateLike;

internal sealed class CreateLikeValidator : AbstractValidator<CreateLikeRequest>
{
    public CreateLikeValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().ValidGuid();
    }
}
