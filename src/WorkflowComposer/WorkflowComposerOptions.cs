using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WorkflowComposer;

// Fluent configuration surface for AddWorkflowComposer. Backend
// packages (e.g. WorkflowComposer.Sqlite) extend this with their
// own UseXxxStore methods.
public sealed class WorkflowComposerOptions
{
    public WorkflowComposerOptions(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    // Workflows go cold once the user walks away from the checkout
    // page. Twenty minutes is long enough that a returning user with
    // a back-button still finds their cart, short enough that
    // abandoned rows don't accumulate forever.
    public TimeSpan InactivityThreshold { get; set; } = TimeSpan.FromMinutes(20);

    // How often the cleanup pass runs. Smaller than the threshold so
    // abandoned workflows get pruned within ~one interval of crossing
    // the line, not at some arbitrary delay after.
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);

    // Discover and register every IWorkflowSlice implementation in
    // the given assemblies. Each slice is registered as a singleton
    // because it carries no per-request state.
    public WorkflowComposerOptions RegisterSlicesFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface) continue;
                if (!typeof(IWorkflowSlice).IsAssignableFrom(type)) continue;
                Services.AddSingleton(typeof(IWorkflowSlice), type);
            }
        }
        return this;
    }

    public WorkflowComposerOptions RegisterSlicesFromAssembliesOf(params Type[] markerTypes)
    {
        var assemblies = markerTypes.Select(t => t.Assembly).Distinct().ToArray();
        return RegisterSlicesFromAssemblies(assemblies);
    }
}
