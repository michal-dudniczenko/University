using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;
using static Soundmates.Api.Common.Constants.ApplicationConstants;

namespace Soundmates.Api.Features.Users.UpdateProfile;

internal sealed class UpdateUserProfileArtistValidator : AbstractValidator<UpdateUserProfileArtistRequest>
{
    public UpdateUserProfileArtistValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(MaximumUserNameLength);
        RuleFor(x => x.Description).MaximumLength(MaximumUserDescriptionLength);
        RuleFor(x => x.CountryId).NotEmpty().ValidGuid();
        RuleFor(x => x.CityId).NotEmpty().ValidGuid();
        RuleFor(x => x.TagsIds).NotNull();
        RuleForEach(x => x.TagsIds).NotEmpty().ValidGuid();
        RuleFor(x => x.MusicSamplesOrder).NotNull();
        RuleForEach(x => x.MusicSamplesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.ProfilePicturesOrder).NotNull();
        RuleForEach(x => x.ProfilePicturesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.BirthDate).BirthDate();
        RuleFor(x => x.GenderId).NotEmpty().ValidGuid();
    }
}

internal sealed class UpdateUserProfileBandValidator : AbstractValidator<UpdateUserProfileBandRequest>
{
    public UpdateUserProfileBandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(MaximumUserNameLength);
        RuleFor(x => x.Description).MaximumLength(MaximumUserDescriptionLength);
        RuleFor(x => x.CountryId).NotEmpty().ValidGuid();
        RuleFor(x => x.CityId).NotEmpty().ValidGuid();
        RuleFor(x => x.TagsIds).NotNull();
        RuleForEach(x => x.TagsIds).NotEmpty().ValidGuid();
        RuleFor(x => x.MusicSamplesOrder).NotNull();
        RuleForEach(x => x.MusicSamplesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.ProfilePicturesOrder).NotNull();
        RuleForEach(x => x.ProfilePicturesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.BandMembers).NotNull();
        RuleFor(x => x.BandMembers).Must(x => x.Count < MaximumBandMembersCount)
            .WithMessage($"Maximum number of band members is: {MaximumBandMembersCount}");
        RuleForEach(x => x.BandMembers).ChildRules(member =>
        {
            member.RuleFor(m => m.Name).NotEmpty().MaximumLength(MaximumBandMemberNameLength);
            member.RuleFor(m => m.Age).InclusiveBetween(MinimumBandMemberAge, MaximumBandMemberAge);
            member.RuleFor(m => m.BandRoleId).NotEmpty().ValidGuid();
        });
    }
}
