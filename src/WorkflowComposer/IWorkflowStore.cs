namespace WorkflowComposer;

// Backend abstraction. The framework reads and writes per-slice JSON
// keyed by (workflowId, sliceKey) and, at submit time, hands a bag of
// outgoing commands to the store for atomic dispatch with whatever
// "submitted" state the slice writers persisted.
//
// Implementations: WorkflowComposer.Sqlite (NServiceBus outbox via
// TransactionalSession), future Redis, in-memory for tests.
public interface IWorkflowStore
{
    Task<object?> ReadSlice(Guid workflowId, string sliceKey, Type sliceType, CancellationToken ct);

    Task WriteSlice(Guid workflowId, string sliceKey, object slice, Type sliceType, CancellationToken ct);

    // Atomic: either every command in `commands` will eventually
    // dispatch, or none. Backends with an outbox commit the workflow
    // row and the queued messages in one transaction.
    Task Submit(Guid workflowId, IReadOnlyList<object> commands, CancellationToken ct);

    // Drop every slice belonging to workflows whose most recent write
    // is older than `inactivity`. Returns the number of rows deleted
    // for telemetry. Backends with native expiry (Redis) implement
    // this as a no-op and lean on TTLs instead.
    Task<int> DeleteInactive(TimeSpan inactivity, CancellationToken ct);
}
