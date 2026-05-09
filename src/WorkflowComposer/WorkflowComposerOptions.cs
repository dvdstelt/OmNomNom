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
