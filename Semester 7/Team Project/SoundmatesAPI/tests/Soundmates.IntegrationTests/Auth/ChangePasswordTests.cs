using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class ChangePasswordTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.ChangePasswordRoute, UriKind.Relative);

    private static ChangePasswordRequest ValidRequest() =>
        new(TestConstants.DefaultPassword, AuthTestConstants.NewValidPassword);

    [Fact]
    public async Task ChangePassword_WithValidOldPassword_Returns204AndRevokesRefreshTokens()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedRefreshTokenAsync(user.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var remaining = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.CountAsync(rt => rt.UserId == user.Id));
        remaining.Should().Be(0);
    }

    [Fact]
    public async Task ChangePassword_EmptyOldPassword_Returns422()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(
            RouteUri, new ChangePasswordRequest("", AuthTestConstants.NewValidPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("OldPassword");
    }

    [Theory]
    [InlineData(AuthTestConstants.PasswordTooShort)]
    [InlineData(AuthTestConstants.PasswordTooLong)]
    [InlineData(AuthTestConstants.PasswordNoLower)]
    [InlineData(AuthTestConstants.PasswordNoUpper)]
    [InlineData(AuthTestConstants.PasswordNoDigit)]
    [InlineData(AuthTestConstants.PasswordNoSpecial)]
    [InlineData(AuthTestConstants.PasswordWithSpace)]
    public async Task ChangePassword_InvalidNewPassword_Returns422(string newPassword)
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(
            RouteUri, new ChangePasswordRequest(TestConstants.DefaultPassword, newPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("NewPassword");
    }

    [Fact]
    public async Task ChangePassword_WrongOldPassword_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(
            RouteUri, new ChangePasswordRequest("WrongOld123!", AuthTestConstants.NewValidPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_FirstLoginUser_Returns204()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            "ChangePassword uses GetUserAsync, not GetAuthorizedUserAsync, so first-login is allowed");
    }

    [Fact]
    public async Task ChangePassword_DeactivatedUserWithValidJwt_Returns204()
    {
        // GetUserAsync does not check IsActive, so a still-valid JWT for a deactivated user reaches
        // the handler and the change succeeds.
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangePassword_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
