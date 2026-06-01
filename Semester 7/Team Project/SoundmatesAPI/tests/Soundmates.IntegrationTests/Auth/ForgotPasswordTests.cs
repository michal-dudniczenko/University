using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class ForgotPasswordTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.ForgotPasswordRoute, UriKind.Relative);

    [Fact]
    public async Task ForgotPassword_ConfirmedUser_SendsResetEmail()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ForgotPasswordRequest(user.Email), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var sent = SentEmails.Should().ContainSingle().Which;
        sent.Kind.Should().Be(CapturedEmailKind.PasswordReset);
        sent.Email.Should().Be(user.Email);
        sent.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ForgotPassword_UnknownEmail_Returns204AndSendsNoEmail()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ForgotPasswordRequest($"nobody-{Guid.NewGuid():N}@test.local"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        SentEmails.Should().BeEmpty();
    }

    [Fact]
    public async Task ForgotPassword_UnconfirmedUser_Returns204AndSendsNoEmail()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ForgotPasswordRequest(user.Email), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        SentEmails.Should().BeEmpty("no reset email for unconfirmed users");
    }

    [Theory]
    [InlineData("")]
    [InlineData(AuthTestConstants.EmailInvalidNoAt)]
    [InlineData(AuthTestConstants.EmailInvalidTrailingAt)]
    public async Task ForgotPassword_InvalidEmail_Returns422(string email)
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new ForgotPasswordRequest(email), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Email");
    }
}
