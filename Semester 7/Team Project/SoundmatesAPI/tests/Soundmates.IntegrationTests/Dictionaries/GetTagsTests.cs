using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Dictionaries.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Dictionaries;

public sealed class GetTagsTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetTags_ReturnsAllTagsOrderedByName()
    {
        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.TagsRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadRequiredAsync<List<TagResponse>>();

        tags.Should().NotBeEmpty("dictionary tables are seeded before tests run");

        tags.Should().AllSatisfy(t =>
        {
            t.Id.Should().NotBeEmpty();
            t.Name.Should().NotBeNullOrWhiteSpace();
            t.TagCategoryId.Should().NotBeEmpty();
        });

        var expectedOrder = await Factory.ExecuteDbContextAsync(db =>
            db.Tags.AsNoTracking().OrderBy(t => t.Name).Select(t => t.Name).ToListAsync());
        tags.Select(t => t.Name).Should().Equal(expectedOrder,
            "tags must be ordered by Name using the database collation");
    }

    [Fact]
    public async Task GetTags_AnonymousRequest_ReturnsOk()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var response = await HttpClient.GetAsync(new Uri(DictionariesTestConstants.TagsRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
