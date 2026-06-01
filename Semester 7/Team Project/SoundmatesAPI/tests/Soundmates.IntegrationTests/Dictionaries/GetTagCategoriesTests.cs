using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetTagCategoriesTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetTagCategories_ReturnsAllCategoriesOrderedByName()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.TagCategoriesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await response.ReadRequiredAsync<List<TagCategoryResponse>>();

        categories.Should().NotBeEmpty("dictionary tables are seeded before tests run");

        categories.Should().AllSatisfy(tc =>
        {
            tc.Id.Should().NotBeEmpty();
            tc.Name.Should().NotBeNullOrWhiteSpace();
        });

        categories.Should().Contain(tc => !tc.IsForBand,
            "at least one artist tag category (IsForBand=false) must be seeded");
        categories.Should().Contain(tc => tc.IsForBand,
            "at least one band tag category (IsForBand=true) must be seeded");

        var sortedNames = categories.Select(tc => tc.Name).OrderBy(n => n).ToList();
        categories.Select(tc => tc.Name).Should().ContainInConsecutiveOrder(sortedNames,
            "tag categories must be ordered by Name ascending");
    }

    [Fact]
    public async Task GetTagCategories_AnonymousRequest_ReturnsOk()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.TagCategoriesRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
