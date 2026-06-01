using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetGendersTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetGenders_ReturnsAllGendersOrderedByName()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.GendersRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var genders = await response.ReadRequiredAsync<List<GenderResponse>>();

        genders.Should().NotBeEmpty("dictionary tables are seeded before tests run");

        var sortedNames = genders.Select(g => g.Name).OrderBy(n => n).ToList();
        genders.Select(g => g.Name).Should().ContainInConsecutiveOrder(sortedNames,
            "genders must be ordered by Name ascending");

        genders.Should().AllSatisfy(g =>
        {
            g.Id.Should().NotBeEmpty();
            g.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetGenders_AnonymousRequest_ReturnsOk()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.GendersRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
