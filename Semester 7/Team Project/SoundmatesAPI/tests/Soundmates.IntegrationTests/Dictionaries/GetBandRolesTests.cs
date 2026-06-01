using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetBandRolesTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetBandRoles_ReturnsAllBandRolesOrderedByName()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.BandRolesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roles = await response.ReadRequiredAsync<List<BandRoleResponse>>();

        roles.Should().NotBeEmpty("dictionary tables are seeded before tests run");

        var sortedNames = roles.Select(r => r.Name).OrderBy(n => n).ToList();
        roles.Select(r => r.Name).Should().ContainInConsecutiveOrder(sortedNames,
            "band roles must be ordered by Name ascending");

        roles.Should().AllSatisfy(r =>
        {
            r.Id.Should().NotBeEmpty();
            r.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetBandRoles_AnonymousRequest_ReturnsOk()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.BandRolesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
