namespace WorkflowComposer;

// Coordinates fan-out at workflow completion. Reads every registered
// slice from the store, asks each boundary to translate its slice
// into a command, and hands the bag of commands to the store for
// atomic dispatch.
//
// Boundaries don't depend on this directly - they implement
// IWorkflowSlice. The submit handler in the gateway (or wherever the
// workflow completes) injects this and calls Submit.
public interface IWorkflowSubmitter
{
    Task Submit(Guid workflowId, CancellationToken ct = default);
}
