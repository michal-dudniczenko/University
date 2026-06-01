using Microsoft.EntityFrameworkCore;

namespace Soundmates.IntegrationTests.Common.Seeding;

internal sealed record SeededCity(Guid Id, Guid CountryId, double Latitude, double Longitude);

/// <summary>
/// Read-only lookups against the seeded dictionary tables (Countries, Cities, Genders, BandRoles,
/// Tags). These tables survive Respawn resets, so their rows are stable for the whole test run,
/// but their GUIDs are generated — always look them up rather than hard-coding ids.
/// </summary>
internal static class DictionaryData
{
    public static Task<Guid> GetAnyCountryIdAsync(this CustomWebApplicationFactory factory) =>
        factory.ExecuteDbContextAsync(db =>
            db.Countries.AsNoTracking().OrderBy(c => c.Name).Select(c => c.Id).FirstAsync());

    public static Task<Guid> GetAnyGenderIdAsync(this CustomWebApplicationFactory factory) =>
        factory.ExecuteDbContextAsync(db =>
            db.Genders.AsNoTracking().OrderBy(g => g.Name).Select(g => g.Id).FirstAsync());

    public static Task<Guid> GetAnyBandRoleIdAsync(this CustomWebApplicationFactory factory) =>
        factory.ExecuteDbContextAsync(db =>
            db.BandRoles.AsNoTracking().OrderBy(r => r.Name).Select(r => r.Id).FirstAsync());

    public static Task<SeededCity> GetAnyCityAsync(this CustomWebApplicationFactory factory) =>
        factory.ExecuteDbContextAsync(db =>
            db.Cities.AsNoTracking().OrderBy(c => c.Name)
                .Select(c => new SeededCity(c.Id, c.CountryId, c.Latitude, c.Longitude))
                .FirstAsync());

    public static Task<List<SeededCity>> GetCitiesAsync(this CustomWebApplicationFactory factory, int count) =>
        factory.ExecuteDbContextAsync(db =>
            db.Cities.AsNoTracking().OrderBy(c => c.Name)
                .Select(c => new SeededCity(c.Id, c.CountryId, c.Latitude, c.Longitude))
                .Take(count).ToListAsync());

    /// <summary>Tag ids whose category is an artist category (IsForBand == false).</summary>
    public static Task<List<Guid>> GetArtistTagIdsAsync(this CustomWebApplicationFactory factory, int count) =>
        factory.ExecuteDbContextAsync(db =>
            db.Tags.AsNoTracking().Where(t => !t.TagCategory.IsForBand)
                .OrderBy(t => t.Name).Select(t => t.Id).Take(count).ToListAsync());

    /// <summary>Tag ids whose category is a band category (IsForBand == true).</summary>
    public static Task<List<Guid>> GetBandTagIdsAsync(this CustomWebApplicationFactory factory, int count) =>
        factory.ExecuteDbContextAsync(db =>
            db.Tags.AsNoTracking().Where(t => t.TagCategory.IsForBand)
                .OrderBy(t => t.Name).Select(t => t.Id).Take(count).ToListAsync());
}
