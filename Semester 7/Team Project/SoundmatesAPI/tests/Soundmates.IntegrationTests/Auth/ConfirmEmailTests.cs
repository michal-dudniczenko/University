using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class ConfirmEmailTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.ConfirmEmailRoute, UriKind.Relative);
    private static readonly Uri RegisterRouteUri = new(AuthTestConstants.RegisterRoute, UriKind.Relative);

    private static string NewEmail() => $"confirm-{Guid.NewGuid():N}@test.local";

    [Fact]
    public async Task ConfirmEmail_ValidToken_CreatesUserWithMatchPreferenceAndDeletesPending()
    {
        var email = NewEmail();
        var rawToken = await Factory.SeedPendingRegistrationAsync(email);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(rawToken), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var (user, prefExists, pendingCount) = await Factory.ExecuteDbContextAsync(async db =>
        {
            var u = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            var pref = !(u is null
) && await db.UserMatchPreferences.AsNoTracking().AnyAsync(p => p.UserId == u.Id);
            var pending = await db.PendingRegistrations.AsNoTracking().CountAsync(p => p.Email == email);
            return (u, pref, pending);
        });

        user.Should().NotBeNull();
        user!.EmailConfirmed.Should().BeTrue();
        user.IsFirstLogin.Should().BeTrue();
        prefExists.Should().BeTrue("a default UserMatchPreference is created on confirm");
        pendingCount.Should().Be(0, "all pending rows for the email are deleted");
    }

    [Fact]
    public async Task ConfirmEmail_EmptyToken_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(""), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Token");
    }

    [Fact]
    public async Task ConfirmEmail_UnknownToken_Returns400()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest("no-such-token"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.ReadStringAsync()).Should().Contain("Invalid token");
    }

    [Fact]
    public async Task ConfirmEmail_ExpiredToken_Returns400()
    {
        var email = NewEmail();
        var rawToken = await Factory.SeedPendingRegistrationAsync(
            email, expiresAt: DateTime.UtcNow.AddMinutes(-5));

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(rawToken), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmEmail_ReusedToken_Returns400()
    {
        var email = NewEmail();
        var rawToken = await Factory.SeedPendingRegistrationAsync(email);

        var first = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(rawToken), TestJson.Options, TestContext.Current.CancellationToken);
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var second = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(rawToken), TestJson.Options, TestContext.Current.CancellationToken);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfirmEmail_ThenRegisterSameEmail_Returns422()
    {
        var email = NewEmail();
        var rawToken = await Factory.SeedPendingRegistrationAsync(email);

        var confirm = await HttpClient.PostAsJsonAsync(
            RouteUri, new ConfirmEmailRequest(rawToken), TestJson.Options, TestContext.Current.CancellationToken);
        confirm.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var register = await HttpClient.PostAsJsonAsync(
            RegisterRouteUri,
            new RegisterRequest(email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        register.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        (await register.ReadStringAsync()).Should().Contain("Email is already in use.");
    }
}
