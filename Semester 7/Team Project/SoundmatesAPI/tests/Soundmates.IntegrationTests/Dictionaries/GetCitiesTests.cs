using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetCitiesTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetCities_ValidCountryIdWithCities_ReturnsOkOrderedByName()
    {
        var seededCity = await Factory.GetAnyCityAsync();
        var countryId = seededCity.CountryId;

        var response = await HttpClient.GetAsync(
            new Uri($"{DictionariesTestConstants.CitiesRoute}?countryId={countryId}", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cities = await response.ReadRequiredAsync<List<CityResponse>>();

        cities.Should().NotBeEmpty("the seeded country must have at least one city");

        cities.Should().AllSatisfy(c =>
        {
            c.Id.Should().NotBeEmpty();
            c.Name.Should().NotBeNullOrWhiteSpace();
            c.CountryId.Should().Be(countryId);
        });

        var sortedNames = cities.Select(c => c.Name).OrderBy(n => n).ToList();
        cities.Select(c => c.Name).Should().ContainInConsecutiveOrder(sortedNames,
            "cities must be ordered by Name ascending");
    }

    [Fact]
    public async Task GetCities_AnonymousRequest_ReturnsOk()
    {
        var seededCity = await Factory.GetAnyCityAsync();
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(
            new Uri($"{DictionariesTestConstants.CitiesRoute}?countryId={seededCity.CountryId}", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCities_ValidCountryIdWithNoCities_ReturnsEmptyList()
    {
        var emptyCitiesCountryId = await Factory.ExecuteDbContextAsync(async db =>
        {
            var country = new Country { Name = "EmptyCitiesCountry_" + Guid.NewGuid().ToString("N") };
            db.Countries.Add(country);
            await db.SaveChangesAsync();
            return country.Id;
        });

        var response = await HttpClient.GetAsync(
            new Uri($"{DictionariesTestConstants.CitiesRoute}?countryId={emptyCitiesCountryId}", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cities = await response.ReadRequiredAsync<List<CityResponse>>();
        cities.Should().BeEmpty("no cities were added for this newly-seeded country");
    }

    [Fact]
    public async Task GetCities_CountryIdNotAGuid_Returns400()
    {
        var response = await HttpClient.GetAsync(
            new Uri($"{DictionariesTestConstants.CitiesRoute}?countryId=not-a-guid", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCities_CountryIdOmitted_Returns400()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.CitiesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCities_NonexistentCountryId_ReturnsEmptyList()
    {
        var nonexistentId = Guid.NewGuid();

        var response = await HttpClient.GetAsync(
            new Uri($"{DictionariesTestConstants.CitiesRoute}?countryId={nonexistentId}", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cities = await response.ReadRequiredAsync<List<CityResponse>>();
        cities.Should().BeEmpty("no cities exist for a nonexistent country");
    }
}
