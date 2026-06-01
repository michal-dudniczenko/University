using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetCountriesTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCountries_ReturnsAllCountriesOrderedByName()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.CountriesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countries = await response.ReadRequiredAsync<List<CountryResponse>>();

        countries.Should().NotBeEmpty("dictionary tables are seeded before tests run");

        var expectedOrder = await Factory.ExecuteDbContextAsync(db =>
            db.Countries.AsNoTracking().OrderBy(c => c.Name).Select(c => c.Name).ToListAsync());
        countries.Select(c => c.Name).Should().Equal(expectedOrder,
            "countries must be ordered by Name using the database collation");

        countries.Should().AllSatisfy(c =>
        {
            c.Id.Should().NotBeEmpty();
            c.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetCountries_AnonymousRequest_ReturnsOk()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.CountriesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
