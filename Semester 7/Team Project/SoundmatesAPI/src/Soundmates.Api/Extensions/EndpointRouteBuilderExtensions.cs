using Soundmates.Api.Features.Auth.LogIn;
using Soundmates.Api.Features.Auth.LogOut;
using Soundmates.Api.Features.Auth.Refresh;
using Soundmates.Api.Features.Auth.Register;
using Soundmates.Api.Features.Dictionaries.GetBandRoles;
using Soundmates.Api.Features.Dictionaries.GetCities;
using Soundmates.Api.Features.Dictionaries.GetCountries;
using Soundmates.Api.Features.Dictionaries.GetGenders;
using Soundmates.Api.Features.Dictionaries.GetTagCategories;
using Soundmates.Api.Features.Dictionaries.GetTags;
using Soundmates.Api.Features.Matching.CreateDislike;
using Soundmates.Api.Features.Matching.CreateLike;
using Soundmates.Api.Features.Matching.GetMatches;
using Soundmates.Api.Features.Matching.GetMatchPreference;
using Soundmates.Api.Features.Matching.GetPotentialMatchesArtists;
using Soundmates.Api.Features.Matching.GetPotentialMatchesBands;
using Soundmates.Api.Features.Matching.MatchExists;
using Soundmates.Api.Features.Matching.Unmatch;
using Soundmates.Api.Features.Matching.UpdateMatchPreference;
using Soundmates.Api.Features.Messages.GetConversation;
using Soundmates.Api.Features.Messages.GetConversationsPreview;
using Soundmates.Api.Features.Messages.SendMessage;
using Soundmates.Api.Features.Messages.ViewConversation;
using Soundmates.Api.Features.MusicSamples.DeleteMusicSample;
using Soundmates.Api.Features.MusicSamples.UploadMusicSample;
using Soundmates.Api.Features.ProfilePictures.DeleteProfilePicture;
using Soundmates.Api.Features.ProfilePictures.UploadProfilePicture;
using Soundmates.Api.Features.Reports.ReportUser;
using Soundmates.Api.Features.Users.ChangePassword;
using Soundmates.Api.Features.Users.DeactivateAccount;
using Soundmates.Api.Features.Users.GetOtherProfile;
using Soundmates.Api.Features.Users.GetSelfProfile;
using Soundmates.Api.Features.Users.UpdateProfile;

namespace Soundmates.Api.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        // Auth
        app.MapRegister();
        app.MapLogIn();
        app.MapRefresh();
        app.MapLogOut();

        // Dictionaries
        app.MapGetCountries();
        app.MapGetCities();
        app.MapGetGenders();
        app.MapGetTags();
        app.MapGetTagCategories();
        app.MapGetBandRoles();

        // Matching
        app.MapGetPotentialMatchesArtists();
        app.MapGetPotentialMatchesBands();
        app.MapGetMatchPreference();
        app.MapUpdateMatchPreference();
        app.MapGetMatches();
        app.MapCreateLike();
        app.MapCreateDislike();
        app.MapMatchExists();
        app.MapUnmatch();

        // Messages
        app.MapGetConversationsPreview();
        app.MapGetConversation();
        app.MapSendMessage();
        app.MapViewConversation();

        // MusicSamples
        app.MapUploadMusicSample();
        app.MapDeleteMusicSample();

        // ProfilePictures
        app.MapUploadProfilePicture();
        app.MapDeleteProfilePicture();

        // Reports
        app.MapReportUser();

        // Users
        app.MapGetSelfProfile();
        app.MapUpdateProfile();
        app.MapGetOtherProfile();
        app.MapDeactivateAccount();
        app.MapChangePassword();

        return app;
    }
}
