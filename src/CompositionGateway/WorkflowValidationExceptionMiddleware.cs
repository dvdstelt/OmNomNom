using WorkflowComposer;

namespace CompositionGateway;

// Catches WorkflowValidationException from anywhere in the request
// pipeline and translates it to a 400 with per-slice errors.
// Genuine failures (database, transactional session, etc.) are not
// caught here and continue to surface as 500 via ASP.NET defaults.
public sealed class WorkflowValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (WorkflowValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                workflowId = ex.WorkflowId,
                errors = ex.Errors
            });
        }
    }
}
