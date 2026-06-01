using FluentValidation;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Users.UpdateProfile;

internal abstract class UpdateUserProfileValidator<T> : AbstractValidator<T>
    where T : UpdateUserProfileRequest
{
    protected UpdateUserProfileValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ApplicationConstants.MaximumUserNameLength);
        RuleFor(x => x.Description).MaximumLength(ApplicationConstants.MaximumUserDescriptionLength);
        RuleFor(x => x.CountryId).NotEmpty().ValidGuid();
        RuleFor(x => x.CityId).NotEmpty().ValidGuid();
        RuleFor(x => x.TagsIds).NotNull();
        RuleForEach(x => x.TagsIds).NotEmpty().ValidGuid().When(x => x.TagsIds is not null);
        RuleFor(x => x.MusicSamplesOrder).NotNull();
        RuleForEach(x => x.MusicSamplesOrder).NotEmpty().ValidGuid().When(x => x.MusicSamplesOrder is not null);
        RuleFor(x => x.ProfilePicturesOrder).NotNull();
        RuleForEach(x => x.ProfilePicturesOrder).NotEmpty().ValidGuid().When(x => x.ProfilePicturesOrder is not null);
    }
}

internal sealed class UpdateUserProfileArtistValidator : UpdateUserProfileValidator<UpdateUserProfileArtistRequest>
{
    public UpdateUserProfileArtistValidator()
    {
        RuleFor(x => x.BirthDate).BirthDate();
        RuleFor(x => x.GenderId).NotEmpty().ValidGuid();
    }
}

internal sealed class UpdateUserProfileBandValidator : UpdateUserProfileValidator<UpdateUserProfileBandRequest>
{
    public UpdateUserProfileBandValidator()
    {
        RuleFor(x => x.BandMembers).NotNull();
        RuleFor(x => x.BandMembers)
            .Must(x => x.Count < ApplicationConstants.MaximumBandMembersCount)
            .When(x => x.BandMembers is not null)
            .WithMessage($"Maximum number of band members is: {ApplicationConstants.MaximumBandMembersCount}");
        RuleForEach(x => x.BandMembers).ChildRules(member =>
        {
            member.RuleFor(m => m.Name).NotEmpty().MaximumLength(ApplicationConstants.MaximumBandMemberNameLength);
            member.RuleFor(m => m.Age).InclusiveBetween(ApplicationConstants.MinimumBandMemberAge, ApplicationConstants.MaximumBandMemberAge);
            member.RuleFor(m => m.BandRoleId).NotEmpty().ValidGuid();
        }).When(x => x.BandMembers is not null);
    }
}
