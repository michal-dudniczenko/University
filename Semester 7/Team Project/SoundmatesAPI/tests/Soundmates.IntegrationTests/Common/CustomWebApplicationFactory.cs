using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using Soundmates.Api.Common.Services;
using Testcontainers.MsSql;

namespace Soundmates.IntegrationTests.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder(TestConstants.DbDockerImageTag)
        .Build();

    private readonly CapturingEmailService _capturingEmailService = new();
    private readonly InMemoryLogSink _logSink = new();
    private int _remoteIpCounter;

    internal InMemoryLogSink LogSink => _logSink;

    private Respawner _respawner = null!;
    private string _testConnectionString = null!;

    internal CapturingEmailService CapturedEmails => _capturingEmailService;

    public async ValueTask InitializeAsync()
    {
        // The upload endpoints write to the relative path "wwwroot/samples" | "wwwroot/images"
        // (resolved against the current working directory). Those folders don't exist under the
        // test host's working directory, so create them up front to avoid DirectoryNotFoundException.
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "samples"));
        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"));

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

    /// <summary>Resets DB rows and clears captured emails. Call from each test's setup.</summary>
    public async ValueTask ResetStateAsync()
    {
        await ResetDatabaseAsync();
        _capturingEmailService.Clear();
    }

    /// <summary>
    /// Creates an HttpClient over https (so Secure cookies round-trip), with auto-redirects off
    /// and a per-client remote IP header. Pass <paramref name="remoteIp"/> to share a rate-limit
    /// bucket across requests; otherwise a unique IP is generated for isolation.
    /// </summary>
    public HttpClient CreateApiClient(string? remoteIp = null)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri(TestConstants.ClientBaseAddress),
            AllowAutoRedirect = false,
            HandleCookies = true
        });

        client.DefaultRequestHeaders.Add(TestConstants.RemoteIpHeaderName, remoteIp ?? NextRemoteIp());
        return client;
    }

    /// <summary>Generates a unique, deterministic-per-call private IPv4 address.</summary>
    public string NextRemoteIp()
    {
        var id = Interlocked.Increment(ref _remoteIpCounter);
        return $"10.{(id >> 16) & 0xFF}.{(id >> 8) & 0xFF}.{id & 0xFF}";
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

        builder.ConfigureLogging(logging =>
            logging.AddProvider(new InMemoryLoggerProvider(_logSink)));

        builder.ConfigureTestServices(services =>
        {
            // Capture emails instead of logging/sending them.
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService>(_capturingEmailService);

            // Let tests control the remote IP seen by the rate limiter.
            services.AddSingleton<IStartupFilter, TestRemoteIpStartupFilter>();

            // WebApplicationFactory turns ThrowOnBadRequest on, so minimal-API parameter-binding
            // failures (malformed/empty JSON bodies, non-parsable query values) surface as a 500
            // instead of the 400 the app returns in production. Restore production behaviour.
            services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);
        });
    }
}
