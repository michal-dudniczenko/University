using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Features.Users.UpdateProfile;

internal sealed class UpdateUserProfileArtistValidator : AbstractValidator<UpdateUserProfileArtistRequest>
{
    public UpdateUserProfileArtistValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(MaxUserNameLength);
        RuleFor(x => x.Description).MaximumLength(MaxUserDescriptionLength);
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
        RuleFor(x => x.Name).NotEmpty().MaximumLength(MaxUserNameLength);
        RuleFor(x => x.Description).MaximumLength(MaxUserDescriptionLength);
        RuleFor(x => x.CountryId).NotEmpty().ValidGuid();
        RuleFor(x => x.CityId).NotEmpty().ValidGuid();
        RuleFor(x => x.TagsIds).NotNull();
        RuleForEach(x => x.TagsIds).NotEmpty().ValidGuid();
        RuleFor(x => x.MusicSamplesOrder).NotNull();
        RuleForEach(x => x.MusicSamplesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.ProfilePicturesOrder).NotNull();
        RuleForEach(x => x.ProfilePicturesOrder).NotEmpty().ValidGuid();
        RuleFor(x => x.BandMembers).NotNull();
        RuleFor(x => x.BandMembers).Must(x => x.Count < MaxBandMembersCount)
            .WithMessage($"Maximum number of band members is: {MaxBandMembersCount}");
        RuleForEach(x => x.BandMembers).ChildRules(member =>
        {
            member.RuleFor(m => m.Name).NotEmpty().MaximumLength(MaxBandMemberNameLength);
            member.RuleFor(m => m.Age).InclusiveBetween(MinBandMemberAge, MaxBandMemberAge);
            member.RuleFor(m => m.BandRoleId).NotEmpty().ValidGuid();
        });
    }
}
