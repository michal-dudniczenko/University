using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class ResetPasswordTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.ResetPasswordRoute, UriKind.Relative);
    private static readonly Uri ForgotPasswordRouteUri = new(AuthTestConstants.ForgotPasswordRoute, UriKind.Relative);
    private static readonly Uri LoginRouteUri = new($"{AuthTestConstants.LoginRoute}?useCookies=false", UriKind.Relative);
    private static readonly Uri RefreshRouteUri = new(AuthTestConstants.RefreshRoute, UriKind.Relative);

    private async Task<string> RequestResetTokenAsync(string email)
    {
        var forgot = await HttpClient.PostAsJsonAsync(
            ForgotPasswordRouteUri, new ForgotPasswordRequest(email), TestJson.Options, TestContext.Current.CancellationToken);
        forgot.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var token = SentEmails.Should().ContainSingle().Which.Token;
        token.Should().NotBeNullOrEmpty();
        return token!;
    }

    [Fact]
    public async Task ResetPassword_ValidToken_ResetsPasswordRevokesTokensAndAllowsLogin()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedRefreshTokenAsync(user.Id);
        var token = await RequestResetTokenAsync(user.Email);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, token, AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var remaining = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.CountAsync(rt => rt.UserId == user.Id));
        remaining.Should().Be(0, "all refresh tokens are revoked on reset");

        var login = await HttpClient.PostAsJsonAsync(
            LoginRouteUri,
            new LoginRequest(user.Email, AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);
        login.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_InvalidEmail_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest("", "token", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task ResetPassword_EmptyToken_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest("user@test.local", "", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Token");
    }

    [Theory]
    [InlineData(AuthTestConstants.PasswordTooShort)]
    [InlineData(AuthTestConstants.PasswordTooLong)]
    [InlineData(AuthTestConstants.PasswordNoLower)]
    [InlineData(AuthTestConstants.PasswordNoUpper)]
    [InlineData(AuthTestConstants.PasswordNoDigit)]
    [InlineData(AuthTestConstants.PasswordNoSpecial)]
    [InlineData(AuthTestConstants.PasswordWithSpace)]
    public async Task ResetPassword_InvalidNewPassword_Returns422(string password)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest("user@test.local", "dGVzdA", password),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("NewPassword");
    }

    [Fact]
    public async Task ResetPassword_UnknownEmail_Returns400()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest("nobody@test.local", "dGVzdA", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.ReadStringAsync()).Should().Contain("Invalid token");
    }

    [Fact]
    public async Task ResetPassword_UnconfirmedUser_Returns400()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, "dGVzdA", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_ValidBase64ButInvalidIdentityToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        // "dGVzdA" is valid base64url ("test") but not a real Identity reset token.
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, "dGVzdA", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResetPassword_NonBase64UrlToken_Returns500()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        // "!!!notbase64!!!" is NOT valid base64url -> Base64Url.DecodeFromChars throws (no try/catch).
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, "!!!notbase64!!!", AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var body = await response.ReadStringAsync();
        body.Should().Contain("traceId");
    }

    [Fact]
    public async Task ResetPassword_AfterReset_OldRefreshTokenIsRevoked()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var rawRefresh = await Factory.SeedRefreshTokenAsync(user.Id);
        var token = await RequestResetTokenAsync(user.Email);

        var reset = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, token, AuthTestConstants.NewValidPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);
        reset.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var refresh = await HttpClient.PostAsJsonAsync(
            RefreshRouteUri, new RefreshRequest(rawRefresh), TestJson.Options, TestContext.Current.CancellationToken);
        refresh.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetPassword_SamePasswordAsOld_Returns204()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await RequestResetTokenAsync(user.Email);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ResetPasswordRequest(user.Email, token, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent, "no password history is enforced");
    }
}
