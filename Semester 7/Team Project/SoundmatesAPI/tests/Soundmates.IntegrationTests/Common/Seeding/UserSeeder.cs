using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Persistence;

namespace Soundmates.IntegrationTests.Common.Seeding;

/// <summary>Options for seeding a <see cref="User"/> in an arbitrary state.</summary>
internal sealed class SeedUserOptions
{
    public string? Email { get; set; }
    public string Password { get; set; } = TestConstants.DefaultPassword;
    public bool EmailConfirmed { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsFirstLogin { get; set; }
    public bool? IsBand { get; set; }
    public string? Name { get; set; }
    public string? ProfileDescription { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? CityId { get; set; }
    public DateTime? DeactivatedAt { get; set; }

    /// <summary>Create a default <see cref="UserMatchPreference"/> row for the user.</summary>
    public bool CreateMatchPreference { get; set; } = true;

    /// <summary>Tag ids attached to the user's profile (User.Tags), used for match ranking.</summary>
    public IReadOnlyList<Guid>? ProfileTagIds { get; set; }
}

/// <summary>
/// Creates users (and their Artist/Band rows, match preference and reactions) directly in the DB
/// via Identity's <see cref="UserManager{T}"/> so password hashes and security stamps are valid.
/// Everything runs in a single DI scope so the UserManager and DbContext share a tracking context.
/// </summary>
internal static class UserSeeder
{
    private static string UniqueEmail(string? email) =>
        email ?? $"user-{Guid.NewGuid():N}@test.local";

    /// <summary>Low-level seed used by all the convenience helpers below.</summary>
    public static Task<TestUser> CreateUserAsync(
        this CustomWebApplicationFactory factory,
        SeedUserOptions options) =>
        factory.ExecuteScopeAsync(async sp =>
        {
            var userManager = sp.GetRequiredService<UserManager<User>>();
            var db = sp.GetRequiredService<ApplicationDbContext>();

            var email = UniqueEmail(options.Email);
            var user = new User
            {
                Email = email,
                UserName = email,
                IsBand = options.IsBand,
                Name = options.Name,
                ProfileDescription = options.ProfileDescription,
                IsFirstLogin = options.IsFirstLogin,
                IsActive = options.IsActive,
                DeactivatedAt = options.DeactivatedAt,
                CountryId = options.CountryId,
                CityId = options.CityId
            };

            var result = await userManager.CreateAsync(user, options.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to seed user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            user.EmailConfirmed = options.EmailConfirmed;

            if (options.ProfileTagIds is { Count: > 0 })
            {
                var tags = await db.Tags.Where(t => options.ProfileTagIds.Contains(t.Id)).ToListAsync();
                foreach (var tag in tags)
                {
                    user.Tags.Add(tag);
                }
            }

            if (options.CreateMatchPreference)
            {
                db.UserMatchPreferences.Add(new UserMatchPreference { UserId = user.Id });
            }

            await db.SaveChangesAsync();

            return new TestUser(user.Id, email, options.Password) { IsBand = options.IsBand };
        });

    /// <summary>Fully onboarded artist (IsFirstLogin=false, IsBand=false, Artist row created).</summary>
    public static async Task<TestUser> CreateOnboardedArtistAsync(
        this CustomWebApplicationFactory factory,
        string? email = null,
        string? name = null,
        Guid? countryId = null,
        Guid? cityId = null,
        Guid? genderId = null,
        DateOnly? birthDate = null,
        IReadOnlyList<Guid>? profileTagIds = null)
    {
        genderId ??= await factory.GetAnyGenderIdAsync();

        // A properly onboarded user always has Country + City populated (UpdateProfile requires
        // them), and the profile/matching read endpoints throw if they are null. Default to a
        // real seeded city (with its country) when the caller didn't specify one.
        (countryId, cityId) = await factory.ResolveLocationAsync(countryId, cityId);

        var testUser = await factory.CreateUserAsync(new SeedUserOptions
        {
            Email = email,
            Name = name ?? "Test Artist",
            IsBand = false,
            IsFirstLogin = false,
            CountryId = countryId,
            CityId = cityId,
            ProfileTagIds = profileTagIds
        });

        await factory.ExecuteDbContextAsync(async db =>
        {
            db.Artists.Add(new Artist
            {
                UserId = testUser.Id,
                GenderId = genderId.Value,
                BirthDate = birthDate ?? new DateOnly(2000, 1, 1)
            });
            await db.SaveChangesAsync();
        });

        return testUser;
    }

    /// <summary>Fully onboarded band (IsFirstLogin=false, IsBand=true, Band + members created).</summary>
    public static async Task<TestUser> CreateOnboardedBandAsync(
        this CustomWebApplicationFactory factory,
        string? email = null,
        string? name = null,
        Guid? countryId = null,
        Guid? cityId = null,
        int memberCount = 3,
        Guid? bandRoleId = null,
        IReadOnlyList<Guid>? profileTagIds = null)
    {
        bandRoleId ??= await factory.GetAnyBandRoleIdAsync();
        (countryId, cityId) = await factory.ResolveLocationAsync(countryId, cityId);

        var testUser = await factory.CreateUserAsync(new SeedUserOptions
        {
            Email = email,
            Name = name ?? "Test Band",
            IsBand = true,
            IsFirstLogin = false,
            CountryId = countryId,
            CityId = cityId,
            ProfileTagIds = profileTagIds
        });

        await factory.ExecuteDbContextAsync(async db =>
        {
            var band = new Band { UserId = testUser.Id };
            for (var i = 0; i < memberCount; i++)
            {
                band.Members.Add(new BandMember
                {
                    Name = $"Member {i + 1}",
                    Age = 25,
                    DisplayOrder = i,
                    BandId = band.Id,
                    BandRoleId = bandRoleId.Value
                });
            }

            db.Bands.Add(band);
            await db.SaveChangesAsync();
        });

        return testUser;
    }

    /// <summary>
    /// Resolves a consistent (CountryId, CityId) pair for an onboarded user. Respects any value the
    /// caller supplied; fills the rest from seeded city data so the country always matches the city.
    /// </summary>
    private static async Task<(Guid, Guid)> ResolveLocationAsync(
        this CustomWebApplicationFactory factory, Guid? countryId, Guid? cityId)
    {
        if (countryId is not null && cityId is not null)
        {
            return (countryId.Value, cityId.Value);
        }

        return await factory.ExecuteDbContextAsync(async db =>
        {
            var query = db.Cities.AsNoTracking().OrderBy(c => c.Name).AsQueryable();
            if (cityId is not null)
            {
                query = db.Cities.AsNoTracking().Where(c => c.Id == cityId.Value);
            }
            else if (countryId is not null)
            {
                query = db.Cities.AsNoTracking().Where(c => c.CountryId == countryId.Value).OrderBy(c => c.Name);
            }

            var city = await query.Select(c => new { c.Id, c.CountryId }).FirstAsync();
            return (countryId ?? city.CountryId, cityId ?? city.Id);
        });
    }

    /// <summary>Confirmed user that has not completed onboarding (IsFirstLogin=true, IsBand=null).</summary>
    public static Task<TestUser> CreateFirstLoginUserAsync(
        this CustomWebApplicationFactory factory, string? email = null) =>
        factory.CreateUserAsync(new SeedUserOptions { Email = email, IsFirstLogin = true });

    /// <summary>Registered but unconfirmed user (EmailConfirmed=false).</summary>
    public static Task<TestUser> CreateUnconfirmedUserAsync(
        this CustomWebApplicationFactory factory, string? email = null) =>
        factory.CreateUserAsync(new SeedUserOptions { Email = email, EmailConfirmed = false, IsFirstLogin = true });

    /// <summary>Onboarded artist that has since been deactivated (IsActive=false).</summary>
    public static async Task<TestUser> CreateDeactivatedUserAsync(
        this CustomWebApplicationFactory factory, string? email = null)
    {
        var user = await factory.CreateOnboardedArtistAsync(email);
        await factory.ExecuteDbContextAsync(async db =>
        {
            await db.Users.Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IsActive, false)
                    .SetProperty(u => u.DeactivatedAt, DateTime.UtcNow));
        });
        return user;
    }

    /// <summary>Admin user in the "Admin" role (role created on demand; Respawn wipes it each test).</summary>
    public static Task<TestUser> CreateAdminUserAsync(
        this CustomWebApplicationFactory factory,
        string? email = null,
        string password = TestConstants.AdminPassword) =>
        factory.ExecuteScopeAsync(async sp =>
        {
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = sp.GetRequiredService<UserManager<User>>();

            if (!await roleManager.RoleExistsAsync(TestConstants.AdminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(TestConstants.AdminRoleName));
            }

            var resolvedEmail = UniqueEmail(email);
            var user = new User
            {
                Email = resolvedEmail,
                UserName = resolvedEmail,
                EmailConfirmed = true,
                IsFirstLogin = false
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to seed admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(user, TestConstants.AdminRoleName);
            return new TestUser(user.Id, resolvedEmail, password);
        });
}
