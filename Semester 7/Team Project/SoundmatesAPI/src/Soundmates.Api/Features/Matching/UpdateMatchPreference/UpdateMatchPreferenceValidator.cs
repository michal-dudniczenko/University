using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Matching.UpdateMatchPreference;

internal sealed class UpdateMatchPreferenceValidator : AbstractValidator<UpdateMatchPreferenceRequest>
{
    public UpdateMatchPreferenceValidator()
    {
        RuleFor(x => x.FilterTagsIds).NotNull();
        RuleForEach(x => x.FilterTagsIds).NotEmpty().ValidGuid();
        RuleFor(x => x.CountryId)
            .Must(s => Guid.TryParse(s, out _))
            .WithMessage("{PropertyName} must be a valid GUID.")
            .When(x => x.CountryId != null);
        RuleFor(x => x.CityId)
            .Must(s => Guid.TryParse(s, out _))
            .WithMessage("{PropertyName} must be a valid GUID.")
            .When(x => x.CityId != null);
        RuleFor(x => x.ArtistGenderId)
            .Must(s => Guid.TryParse(s, out _))
            .WithMessage("{PropertyName} must be a valid GUID.")
            .When(x => x.ArtistGenderId != null);
    }
}
