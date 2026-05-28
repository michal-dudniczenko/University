using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;

namespace Soundmates.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder(TestConstants.DbDockerImageTag)
        .Build();

    private Respawner _respawner = null!;
    private string _testConnectionString = null!;

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _testConnectionString = new SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            InitialCatalog = TestConstants.TestDatabaseName
        }.ConnectionString;

        // Triggers host startup, which causes Program.cs to run EF Core migrations
        // (InitializeMigrateDatabaseAsync) and dictionary data seeding (UseAsyncSeeding).
        _ = Server;

        await using var connection = new SqlConnection(_testConnectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore =
            [
                new Table("__EFMigrationsHistory"),
                .. TestConstants.DictionaryTableNames.Select(name => new Table(name))
            ]
        });
    }

    public async ValueTask ResetDatabaseAsync()
    {
        await using var connection = new SqlConnection(_testConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseSetting("ConnectionStrings:DefaultConnection", _testConnectionString);
    }
}
