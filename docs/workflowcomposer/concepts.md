# Concepts

Three interfaces in WorkflowComposer's core. Everything else is plumbing.

## `IWorkflowStore`

The backend abstraction. The framework reads and writes per-slice JSON keyed by `(workflowId, sliceKey)`, and at submit time hands a bag of outgoing commands to the store for atomic dispatch.

```csharp
public interface IWorkflowStore
{
    Task<object?> ReadSlice(Guid workflowId, string sliceKey, Type sliceType, CancellationToken ct);
    Task WriteSlice(Guid workflowId, string sliceKey, object slice, Type sliceType, CancellationToken ct);
    Task Submit(Guid workflowId, IReadOnlyList<object> commands, CancellationToken ct);
}
```

Boundaries don't call this typed API directly; they use the typed `Read<T>` / `Write<T>` extension methods.

The contract `Submit` must honour: **either every command in `commands` will eventually dispatch, or none.** Backends with an outbox commit the workflow row and the queued messages in one transaction. Backends without that property aren't reliable submit backends.

## `IWorkflowSlice`

What each service boundary contributes. Two responsibilities: declare the slice's CLR type so the framework knows what to (de)serialize, and translate the persisted slice into a per-boundary command at submit time.

```csharp
public interface IWorkflowSlice
{
    string SliceKey { get; }
    Type SliceType { get; }
    object? BuildSubmitCommand(Guid workflowId, object slice);
}

public abstract class WorkflowSlice<TSlice> : IWorkflowSlice
    where TSlice : class
{
    public abstract string SliceKey { get; }
    public Type SliceType => typeof(TSlice);
    protected abstract object? BuildSubmitCommand(Guid workflowId, TSlice slice);
}
```

Conventions:

- **`SliceKey`** is namespaced by boundary. `Catalog.Cart`, `Finance.BillingAddress`, `Shipping.DeliveryOption`. The framework only requires uniqueness; the convention is for legibility.
- **The slice payload is plain data**, not a domain entity. Use records or POCOs that are safe to serialise.
- **`BuildSubmitCommand` returns `null` to skip a slice.** Useful when a boundary's slice is optional and was never written for this workflow.

## `IWorkflowSubmitter`

The fan-out coordinator. Reads every registered slice, asks each boundary to translate its slice into a command, and hands the resulting bundle to the store.

```csharp
public interface IWorkflowSubmitter
{
    Task Submit(Guid workflowId, CancellationToken ct = default);
}
```

The default implementation just iterates registered `IWorkflowSlice` instances and skips slices that returned `null` from `ReadSlice` or `BuildSubmitCommand`. Order is registration order; if your downstream depends on a specific dispatch order, make sure registration is deterministic.

## Lifetimes

The default registrations:

| Service | Lifetime | Why |
|---|---|---|
| `IWorkflowSlice` (each boundary's contribution) | Singleton | No per-request state; just metadata + a `BuildSubmitCommand` method. |
| `IWorkflowStore` (SQLite backend) | Scoped | Depends on `ITransactionalSession`, which is scoped per request and unusable after `Commit`. |
| `IWorkflowSubmitter` | Scoped | Depends on the scoped store. |

In-memory backends (e.g. for tests) can register the store as a singleton; the submitter follows.

## When the framework opens the session

`SqliteWorkflowStore` only opens the `ITransactionalSession` inside `Submit`. Plain slice reads and writes use a fresh `SqliteConnection` against the same file, with no session involved. This keeps the per-page handlers free of NServiceBus machinery and saves an outbox round-trip on every keystroke.

The implication: if you call `Submit` more than once per HTTP request, the second call will fail because the session is already committed. In normal usage, one workflow completion per request, this is fine.

## What `Submit` writes besides the outbox

The SQLite store records a `$submitted` row in the workflow table inside the same transaction as the outbox commit. That row exists so reads after submit can distinguish "this checkout has been handed off" from "this checkout doesn't exist". The framework doesn't surface it as a slice; it's an implementation detail of the SQLite backend.

## What the framework deliberately doesn't expose

- **No HTTP routing.** WorkflowComposer doesn't add new HTTP route concepts. Service boundaries register their handlers with whatever they're already using (ServiceComposer in this codebase).
- **No retry policy.** Once `Submit` returns, the outbox dispatcher (NServiceBus, in the SQLite backend) owns retries.
- **No saga orchestration.** Sagas live in the per-boundary endpoints. WorkflowComposer's job ends when the bag of commands is enqueued.
- **No cross-workflow operations.** Slice keys are namespaced under one workflow id. Joining state across workflows is application-level.
