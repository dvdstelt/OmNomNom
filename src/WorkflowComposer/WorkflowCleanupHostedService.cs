using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkflowComposer;

internal sealed class WorkflowCleanupHostedService(
    IServiceScopeFactory scopeFactory,
    WorkflowCleanupSettings settings,
    ILogger<WorkflowCleanupHostedService> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(settings.CleanupInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await timer.WaitForNextTickAsync(stoppingToken)) return;
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                // IWorkflowStore is scoped (the SQLite backend depends
                // on the per-request ITransactionalSession), so each
                // pass gets its own scope. DeleteInactive itself
                // doesn't touch the session.
                using var scope = scopeFactory.CreateScope();
                var store = scope.ServiceProvider.GetRequiredService<IWorkflowStore>();
                var deleted = await store.DeleteInactive(settings.InactivityThreshold, stoppingToken);
                if (deleted > 0)
                {
                    log.LogInformation(
                        "Workflow cleanup removed {Deleted} slice rows older than {Threshold}.",
                        deleted, settings.InactivityThreshold);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                // One failed sweep shouldn't take the service down -
                // the next tick will retry naturally.
                log.LogError(ex, "Workflow cleanup pass failed.");
            }
        }
    }
}

internal sealed record WorkflowCleanupSettings(TimeSpan InactivityThreshold, TimeSpan CleanupInterval);
