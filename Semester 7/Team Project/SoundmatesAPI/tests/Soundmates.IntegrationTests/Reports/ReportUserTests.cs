using Soundmates.IntegrationTests.Reports.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Reports;

public sealed class ReportUserTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(ReportsTestConstants.ReportUserRoute, UriKind.Relative);

    [Fact]
    public async Task ReportUser_ValidRequest_SendsModerationEmailWithAllFields()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var email = SentEmails.Should().ContainSingle(e => e.Kind == CapturedEmailKind.Generic).Which;
        email.Email.Should().Be(TestConstants.ModerationEmail);
        email.Body.Should().NotBeNullOrEmpty();
        email.Body.Should().Contain(reporter.Id.ToString());
        email.Body.Should().Contain(reported.Id.ToString());
        email.Body.Should().Contain(ReportsTestConstants.DefaultReason);
        email.Body.Should().Contain(ReportsTestConstants.DefaultDescription);
    }

    [Fact]
    public async Task ReportUser_EmptyReportedUserId_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest("", ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "ReportedUserId");
    }

    [Fact]
    public async Task ReportUser_NonGuidReportedUserId_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest("not-a-guid", ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "ReportedUserId");
    }

    [Fact]
    public async Task ReportUser_EmptyReason_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), "", ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Reason");
    }

    [Fact]
    public async Task ReportUser_ReasonOverMaxLength_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.ReasonOverMaxLength(), ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Reason");
    }

    [Fact]
    public async Task ReportUser_ReasonAtMaxLength_Returns200()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.ReasonAtMaxLength(), ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReportUser_EmptyDescription_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ""),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Description");
    }

    [Fact]
    public async Task ReportUser_DescriptionOverMaxLength_Returns422()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DescriptionOverMaxLength()),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        await AssertErrorKeyAsync(response, "Description");
    }

    [Fact]
    public async Task ReportUser_DescriptionAtMaxLength_Returns200()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DescriptionAtMaxLength()),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReportUser_NonexistentReportedUser_Returns200()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);
        var phantomId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(phantomId.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReportUser_ReportSelf_Returns200()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reporter.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReportUser_HtmlInFields_IsEncodedInEmailBody()
    {
        var reporter = await Factory.CreateOnboardedArtistAsync();
        var reported = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(reporter);

        const string htmlReason = "<script>alert('xss')</script>";
        const string htmlDescription = "<b>Bold claim</b> & <i>italic</i>";

        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), htmlReason, htmlDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var email = SentEmails.Should().ContainSingle(e => e.Kind == CapturedEmailKind.Generic).Which;
        // Source HTML-encodes user input; raw tags must not appear verbatim.
        email.Body.Should().NotContain("<script>", "HTML input must be encoded, not rendered verbatim");
        email.Body.Should().Contain("&lt;", "HTML encoding must convert '<' to '&lt;'");
    }

    [Fact]
    public async Task ReportUser_NoCredentials_Returns401()
    {
        var reported = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReportUser_DeletedCaller_Returns401()
    {
        var deletedUserId = Guid.NewGuid();
        var token = await Factory.MintTokenAsync(deletedUserId, "ghost@test.local");
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var reported = await Factory.CreateOnboardedArtistAsync();
        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReportUser_UnconfirmedCaller_Returns401()
    {
        var unconfirmed = await Factory.CreateUnconfirmedUserAsync();
        var token = await Factory.GetAccessTokenAsync(unconfirmed.Id);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var reported = await Factory.CreateOnboardedArtistAsync();
        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReportUser_DeactivatedCaller_Returns401()
    {
        var deactivated = await Factory.CreateDeactivatedUserAsync();
        var token = await Factory.GetAccessTokenAsync(deactivated.Id);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var reported = await Factory.CreateOnboardedArtistAsync();
        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReportUser_FirstLoginCaller_Returns401()
    {
        var firstLogin = await Factory.CreateFirstLoginUserAsync();
        var token = await Factory.GetAccessTokenAsync(firstLogin.Id);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var reported = await Factory.CreateOnboardedArtistAsync();
        var response = await client.PostAsJsonAsync(
            RouteUri,
            new ReportUserRequest(reported.Id.ToString(), ReportsTestConstants.DefaultReason, ReportsTestConstants.DefaultDescription),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static async Task AssertErrorKeyAsync(HttpResponseMessage response, string key)
    {
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(key);
    }
}
