using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Matching.CreateDislike;

internal sealed class CreateDislikeValidator : AbstractValidator<CreateDislikeRequest>
{
    public CreateDislikeValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().ValidGuid();
    }
}
