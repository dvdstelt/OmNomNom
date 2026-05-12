namespace WorkflowComposer.Sqlite;

public sealed class SqliteWorkflowStoreOptions
{
    public required string ConnectionString { get; init; }

    // The NServiceBus endpoint that hosts the TransactionalSession
    // dispatcher. The gateway is SendOnly, so a separate (typically
    // tiny) endpoint handles the control message and dispatches the
    // outbox. Pass its endpoint name here.
    public required string ProcessorEndpoint { get; init; }

    public string TableName { get; init; } = "WorkflowSlices";
}
