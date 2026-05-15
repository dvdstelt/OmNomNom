using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WorkflowComposer;

// Catches WorkflowValidationException from anywhere in the request
// pipeline and translates it to a 400 ValidationProblemDetails
// (RFC 7807) carrying per-slice errors. Genuine failures (database,
// transactional session, etc.) are not caught here and continue to
// surface as 500 via ASP.NET defaults.
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
            var problem = new ValidationProblemDetails(
                ex.Errors.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Workflow submit failed validation.",
                Detail = $"Workflow {ex.WorkflowId} could not be submitted; one or more slices reported errors.",
                Instance = context.Request.Path
            };
            problem.Extensions["workflowId"] = ex.WorkflowId;

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
