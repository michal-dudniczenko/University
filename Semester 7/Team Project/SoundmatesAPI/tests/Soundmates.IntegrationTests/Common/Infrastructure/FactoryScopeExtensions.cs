using Microsoft.Extensions.DependencyInjection;
using Soundmates.Api.Persistence;

namespace Soundmates.IntegrationTests.Common.Infrastructure;

/// <summary>
/// Helpers for running code inside a DI scope against the running test host — the canonical way
/// to seed/inspect the database or resolve scoped services (UserManager, ApplicationDbContext, …).
/// </summary>
internal static class FactoryScopeExtensions
{
    public static async Task ExecuteScopeAsync(
        this CustomWebApplicationFactory factory,
        Func<IServiceProvider, Task> action)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        await action(scope.ServiceProvider);
    }

    public static async Task<T> ExecuteScopeAsync<T>(
        this CustomWebApplicationFactory factory,
        Func<IServiceProvider, Task<T>> action)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        return await action(scope.ServiceProvider);
    }

    public static Task ExecuteDbContextAsync(
        this CustomWebApplicationFactory factory,
        Func<ApplicationDbContext, Task> action) =>
        factory.ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));

    public static Task<T> ExecuteDbContextAsync<T>(
        this CustomWebApplicationFactory factory,
        Func<ApplicationDbContext, Task<T>> action) =>
        factory.ExecuteScopeAsync(sp => action(sp.GetRequiredService<ApplicationDbContext>()));
}
