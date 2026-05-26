using Soundmates.Api.Features.Auth.ChangePassword;
using Soundmates.Api.Features.Auth.ConfirmEmail;
using Soundmates.Api.Features.Auth.CsrfToken;
using Soundmates.Api.Features.Auth.DeactivateAccount;
using Soundmates.Api.Features.Auth.ForgotPassword;
using Soundmates.Api.Features.Auth.Login;
using Soundmates.Api.Features.Auth.Logout;
using Soundmates.Api.Features.Auth.Refresh;
using Soundmates.Api.Features.Auth.Register;
using Soundmates.Api.Features.Auth.ResendEmailConfirmation;
using Soundmates.Api.Features.Auth.ResetPassword;
using Soundmates.Api.Features.Auth.RevokeAllTokens;
using Soundmates.Api.Features.Auth.RevokeToken;
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
using Soundmates.Api.Features.Reports.BlockUser;
using Soundmates.Api.Features.Reports.ReportUser;
using Soundmates.Api.Features.Users.GetOtherUserProfile;
using Soundmates.Api.Features.Users.GetSelfProfile;
using Soundmates.Api.Features.Users.UpdateProfile;

namespace Soundmates.Api.Extensions;

internal static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        // Auth
        app.MapChangePassword();
        app.MapConfirmEmail();
        app.MapCsrfToken();
        app.MapDeactivateAccount();
        app.MapForgotPassword();
        app.MapLogIn();
        app.MapLogOut();
        app.MapRefresh();
        app.MapRegister();
        app.MapRevokeToken();
        app.MapRevokeAllTokens();
        app.MapResendEmailConfirmation();
        app.MapResetPassword();

        // Dictionaries
        app.MapGetBandRoles();
        app.MapGetCities();
        app.MapGetCountries();
        app.MapGetGenders();
        app.MapGetTagCategories();
        app.MapGetTags();

        // Matching
        app.MapCreateDislike();
        app.MapCreateLike();
        app.MapGetMatches();
        app.MapGetMatchPreference();
        app.MapGetPotentialMatchesArtists();
        app.MapGetPotentialMatchesBands();
        app.MapMatchExists();
        app.MapUpdateMatchPreference();
        app.MapUnmatch();

        // Messages
        app.MapGetConversation();
        app.MapGetConversationsPreview();
        app.MapSendMessage();
        app.MapViewConversation();

        // MusicSamples
        app.MapDeleteMusicSample();
        app.MapUploadMusicSample();

        // ProfilePictures
        app.MapDeleteProfilePicture();
        app.MapUploadProfilePicture();

        // Reports
        app.MapBlockUser();
        app.MapReportUser();

        // Users
        app.MapGetOtherUserProfile();
        app.MapGetSelfProfile();
        app.MapUpdateProfile();

        return app;
    }
}
