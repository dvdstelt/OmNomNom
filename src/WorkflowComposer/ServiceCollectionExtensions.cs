using Microsoft.Extensions.DependencyInjection;

namespace WorkflowComposer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowComposer(
        this IServiceCollection services,
        Action<WorkflowComposerOptions> configure)
    {
        var options = new WorkflowComposerOptions(services);
        configure(options);

        // Scoped because the SQLite backend's IWorkflowStore is scoped
        // (it depends on the per-request ITransactionalSession). An
        // in-memory store would be fine as singleton, but matching the
        // store's lifetime avoids captive-dependency surprises.
        services.AddScoped<IWorkflowSubmitter, WorkflowSubmitter>();

        return services;
    }
}
