namespace ITOps.Shared.EndpointConfiguration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Base class for per-endpoint database seeders. Each endpoint's
// AddXEndpoint(...) extension registers a concrete subclass as a hosted
// service so EnsureCreatedAsync + seed runs in the host's StartingAsync
// phase, before any IHostedService.StartAsync. That means the schema and
// demo data are in place before NServiceBus starts the message pump, and
// the caller's Program.cs doesn't need a separate await InitializeAsync.
public abstract class DatabaseSeederHostedService(IServiceProvider services) : IHostedLifecycleService
{
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        await SeedAsync(scope.ServiceProvider, cancellationToken);
    }

    protected abstract Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken);

    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
