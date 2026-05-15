using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;

namespace Soundmates.Api.Persistence;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Band> Bands => Set<Band>();
    public DbSet<BandMember> BandMembers => Set<BandMember>();
    public DbSet<BandRole> BandRoles => Set<BandRole>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TagCategory> TagCategories => Set<TagCategory>();
    public DbSet<MusicSample> MusicSamples => Set<MusicSample>();
    public DbSet<ProfilePicture> ProfilePictures => Set<ProfilePicture>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Dislike> Dislikes => Set<Dislike>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Gender> Genders => Set<Gender>();
    public DbSet<UserMatchPreference> UserMatchPreferences => Set<UserMatchPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
