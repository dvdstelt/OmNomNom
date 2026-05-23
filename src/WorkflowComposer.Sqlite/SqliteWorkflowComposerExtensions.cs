using Messaging.Persistence.Sqlite;
using Messaging.Persistence.Sqlite.TransactionalSession;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.TransactionalSession;

namespace WorkflowComposer.Sqlite;

public static class SqliteWorkflowComposerExtensions
{
    // Wires the SQLite-backed store + the matching NServiceBus
    // persistence + outbox + transactional session feature on the
    // gateway endpoint configuration. Call this from the host that
    // owns the endpoint configuration before UseNServiceBus.
    public static WorkflowComposerOptions UseSqliteStore(
        this WorkflowComposerOptions options,
        EndpointConfiguration endpointConfiguration,
        SqliteWorkflowStoreOptions storeOptions)
    {
        ArgumentNullException.ThrowIfNull(endpointConfiguration);
        ArgumentNullException.ThrowIfNull(storeOptions);

        var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
        persistence.ConnectionString(storeOptions.ConnectionString);
        persistence.EnableTransactionalSession(new TransactionalSessionOptions
        {
            ProcessorEndpoint = storeOptions.ProcessorEndpoint
        });
        endpointConfiguration.EnableOutbox();

        options.Services.AddSingleton(storeOptions);
        // Scoped: ITransactionalSession is per-request and is not
        // reusable after Commit. Store and Submitter follow the same
        // lifetime so each HTTP request gets a fresh session.
        options.Services.AddScoped<IWorkflowStore, SqliteWorkflowStore>();

        return options;
    }
}
