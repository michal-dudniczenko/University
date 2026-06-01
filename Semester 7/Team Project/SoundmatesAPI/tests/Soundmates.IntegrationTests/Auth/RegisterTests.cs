using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class RegisterTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.RegisterRoute, UriKind.Relative);

    private static string NewEmail() => $"reg-{Guid.NewGuid():N}@test.local";

    [Fact]
    public async Task Register_ValidRequest_CreatesPendingRowAndSendsConfirmationEmail()
    {
        var email = NewEmail();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var pending = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking()
            .Where(pr => pr.Email == email)
            .ToListAsync());

        pending.Should().HaveCount(1);
        pending[0].PasswordHash.Should().NotBeNullOrEmpty()
            .And.NotBe(TestConstants.DefaultPassword);
        pending[0].EmailTokenHash.Should().NotBeNullOrEmpty();
        pending[0].ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(2));

        var captured = SentEmails.Should().ContainSingle().Which;
        captured.Kind.Should().Be(CapturedEmailKind.RegistrationConfirmation);
        captured.Email.Should().Be(email);
        captured.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_PurgesExpiredPendingRows()
    {
        var expiredEmail = NewEmail();
        await Factory.SeedPendingRegistrationAsync(
            expiredEmail, expiresAt: DateTime.UtcNow.AddMinutes(-5));

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(NewEmail(), TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var expiredRows = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking()
            .Where(pr => pr.Email == expiredEmail)
            .ToListAsync());

        expiredRows.Should().BeEmpty("expired pending registrations are purged before insert");
    }

    [Fact]
    public async Task Register_SameUnconfirmedEmail_CreatesSecondPendingRow()
    {
        var email = NewEmail();

        var first = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var second = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);
        second.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var pending = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking()
            .Where(pr => pr.Email == email)
            .ToListAsync());

        pending.Should().HaveCount(2, "validator only checks confirmed Users, not pending rows");
    }

    [Fact]
    public async Task Register_EmptyEmail_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest("", TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Email");
    }

    [Fact]
    public async Task Register_EmailTooLong_Returns422()
    {
        var longEmail = new string('a', 95) + "@test.local"; // > 100 chars

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(longEmail, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Email");
    }

    [Theory]
    [InlineData(AuthTestConstants.EmailInvalidNoAt)]
    [InlineData(AuthTestConstants.EmailInvalidTrailingAt)]
    [InlineData(AuthTestConstants.EmailInvalidLeadingAt)]
    public async Task Register_InvalidEmailFormat_Returns422(string email)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Email");
    }

    [Fact]
    public async Task Register_AlreadyConfirmedEmail_Returns422()
    {
        var existing = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(existing.Email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var body = await response.ReadStringAsync();
        body.Should().Contain("Email is already in use.");
    }

    [Theory]
    [InlineData(AuthTestConstants.PasswordTooShort)]
    [InlineData(AuthTestConstants.PasswordTooLong)]
    [InlineData(AuthTestConstants.PasswordWithSpace)]
    [InlineData(AuthTestConstants.PasswordWithAccent)]
    [InlineData(AuthTestConstants.PasswordNoLower)]
    [InlineData(AuthTestConstants.PasswordNoUpper)]
    [InlineData(AuthTestConstants.PasswordNoDigit)]
    [InlineData(AuthTestConstants.PasswordNoSpecial)]
    public async Task Register_InvalidPassword_Returns422(string password)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(NewEmail(), password), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Password");
    }

    [Theory]
    [InlineData(AuthTestConstants.ValidPasswordMin8)]
    [InlineData(AuthTestConstants.ValidPasswordMax32)]
    public async Task Register_BoundaryValidPassword_Returns204(string password)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(NewEmail(), password), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Register_EmptyEmail_ReturnsSingleEmailError()
    {
        // Empty email: Cascade.Stop on Email means only NotEmpty fires (no chained email errors),
        // but Password is a separate rule chain so it may still report. Assert exactly one Email error.
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest("", TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Email");
        problem.Errors["Email"].Should().HaveCount(1, "Cascade.Stop yields a single email error");
    }

    [Fact]
    public async Task Register_PasswordStoredHashed_NotInPlaintext()
    {
        var email = NewEmail();
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RegisterRequest(email, TestConstants.DefaultPassword), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await response.ReadStringAsync()).Should().BeEmpty();

        var hash = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking()
            .Where(pr => pr.Email == email)
            .Select(pr => pr.PasswordHash)
            .FirstAsync());
        hash.Should().NotContain(TestConstants.DefaultPassword);
    }

    private static async Task AssertErrorKeyAsync(HttpResponseMessage response, string key)
    {
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(key);
    }
}
