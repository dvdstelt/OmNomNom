namespace WorkflowComposer;

// Thrown by WorkflowSubmitter when one or more slices fail their
// Validate() check at submit time. Errors are grouped by SliceKey
// so callers (typically a global middleware) can surface a 400
// response with per-slice detail.
public sealed class WorkflowValidationException(
    Guid workflowId,
    IReadOnlyDictionary<string, IReadOnlyList<string>> errors)
    : Exception($"Workflow {workflowId} failed validation across {errors.Count} slice(s).")
{
    public Guid WorkflowId { get; } = workflowId;

    public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; } = errors;
}
