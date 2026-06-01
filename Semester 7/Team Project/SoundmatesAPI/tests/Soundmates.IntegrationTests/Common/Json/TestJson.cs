using System.Net.Http.Json;
using System.Text.Json;

namespace Soundmates.IntegrationTests.Common.Json;

/// <summary>
/// Shared JSON settings + helpers matching ASP.NET Core's default web serialization
/// (camelCase property names, case-insensitive reads).
/// </summary>
internal static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static Task<T?> ReadAsync<T>(this HttpResponseMessage response) =>
        response.Content.ReadFromJsonAsync<T>(Options);

    public static async Task<T> ReadRequiredAsync<T>(this HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(Options);
        value.Should().NotBeNull("response body was expected to deserialize to {0}", typeof(T).Name);
        return value!;
    }

    public static async Task<string> ReadStringAsync(this HttpResponseMessage response) =>
        await response.Content.ReadAsStringAsync();
}
