using Microsoft.AspNetCore.Builder;

namespace WorkflowComposer;

public static class ApplicationBuilderExtensions
{
    // Wires the WorkflowComposer-owned request-pipeline pieces.
    // Today: the WorkflowValidationException -> 400 translator.
    // Call from the host's pipeline configuration, e.g.:
    //     app.UseWorkflowComposer();
    //     app.UseRouting();
    public static IApplicationBuilder UseWorkflowComposer(this IApplicationBuilder app)
    {
        app.UseMiddleware<WorkflowValidationExceptionMiddleware>();
        return app;
    }
}
