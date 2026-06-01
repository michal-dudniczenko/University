using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class ResendEmailConfirmationTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.ResendEmailRoute, UriKind.Relative);
    private static readonly Uri ConfirmEmailRouteUri = new(AuthTestConstants.ConfirmEmailRoute, UriKind.Relative);

    private static string NewEmail() => $"resend-{Guid.NewGuid():N}@test.local";

    [Fact]
    public async Task ResendEmail_ExistingPendingRow_RotatesTokenAndSendsEmail()
    {
        var email = NewEmail();
        var oldToken = await Factory.SeedPendingRegistrationAsync(email);

        var oldHash = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking().Where(p => p.Email == email).Select(p => p.EmailTokenHash).FirstAsync());

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ResendEmailConfirmationRequest(email), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var (newHash, newExpiry) = await Factory.ExecuteDbContextAsync(db => db.PendingRegistrations
            .AsNoTracking().Where(p => p.Email == email)
            .Select(p => new ValueTuple<byte[], DateTime>(p.EmailTokenHash, p.ExpiresAt))
            .FirstAsync());

        newHash.Should().NotEqual(oldHash, "the email token hash is rotated");
        newExpiry.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(2));

        var sent = SentEmails.Should().ContainSingle().Which;
        sent.Kind.Should().Be(CapturedEmailKind.RegistrationConfirmation);
        sent.Email.Should().Be(email);

        // Old raw token no longer confirms.
        var confirmOld = await HttpClient.PostAsJsonAsync(
            ConfirmEmailRouteUri, new ConfirmEmailRequest(oldToken), TestJson.Options, TestContext.Current.CancellationToken);
        confirmOld.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendEmail_NoPendingRow_Returns204AndSendsNoEmail()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ResendEmailConfirmationRequest(NewEmail()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        SentEmails.Should().BeEmpty("enumeration protection: no email for unknown addresses");
    }

    [Theory]
    [InlineData("")]
    [InlineData(AuthTestConstants.EmailInvalidNoAt)]
    [InlineData(AuthTestConstants.EmailInvalidTrailingAt)]
    public async Task ResendEmail_InvalidEmail_Returns422(string email)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ResendEmailConfirmationRequest(email), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task ResendEmail_EmailTooLong_Returns422()
    {
        var longEmail = new string('a', 95) + "@test.local";
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ResendEmailConfirmationRequest(longEmail), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
