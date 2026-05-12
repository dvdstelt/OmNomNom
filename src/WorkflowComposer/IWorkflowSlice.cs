namespace WorkflowComposer;

// Implemented by each service boundary that participates in the
// workflow. The framework uses this to (1) know what types it should
// (de)serialize for the boundary's slice and (2) translate the slice
// into a per-boundary command at submit time.
public interface IWorkflowSlice
{
    // Unique key per slice. Boundary-namespaced by convention,
    // e.g. "Catalog.Cart", "Finance.BillingAddress".
    string SliceKey { get; }

    // The CLR type the framework should (de)serialize for this slice.
    Type SliceType { get; }

    // Translate the persisted slice into the message that gets sent
    // to the boundary's endpoint when the workflow submits. Return
    // null to skip (e.g., the slice was never written).
    object? BuildSubmitCommand(Guid workflowId, object slice);
}

// Strongly-typed convenience base class. Boundaries usually inherit
// from this rather than implementing IWorkflowSlice directly.
public abstract class WorkflowSlice<TSlice> : IWorkflowSlice
    where TSlice : class
{
    public abstract string SliceKey { get; }

    public Type SliceType => typeof(TSlice);

    public object? BuildSubmitCommand(Guid workflowId, object slice) =>
        BuildSubmitCommand(workflowId, (TSlice)slice);

    protected abstract object? BuildSubmitCommand(Guid workflowId, TSlice slice);
}
