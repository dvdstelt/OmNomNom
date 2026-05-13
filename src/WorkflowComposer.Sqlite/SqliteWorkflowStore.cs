using System.Text.Json;
using Messaging.Persistence.Sqlite;
using Messaging.Persistence.Sqlite.TransactionalSession;
using Microsoft.Data.Sqlite;
using NServiceBus;
using NServiceBus.TransactionalSession;

namespace WorkflowComposer.Sqlite;

internal sealed class SqliteWorkflowStore(
    SqliteWorkflowStoreOptions options,
    ITransactionalSession session) : IWorkflowStore
{
    static bool tableEnsured;
    static readonly SemaphoreSlim ensureLock = new(1, 1);

    public async Task<object?> ReadSlice(Guid workflowId, string sliceKey, Type sliceType, CancellationToken ct)
    {
        await EnsureTable(ct);

        await using var connection = new SqliteConnection(options.ConnectionString);
        await connection.OpenAsync(ct);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"SELECT SliceJson FROM {options.TableName} WHERE WorkflowId = $id AND SliceKey = $key";
        cmd.Parameters.AddWithValue("$id", workflowId.ToString());
        cmd.Parameters.AddWithValue("$key", sliceKey);

        var raw = await cmd.ExecuteScalarAsync(ct);
        if (raw is null or DBNull) return null;

        var json = (string)raw;
        return JsonSerializer.Deserialize(json, sliceType);
    }

    public async Task WriteSlice(Guid workflowId, string sliceKey, object slice, Type sliceType, CancellationToken ct)
    {
        // Slice writes happen on every checkout-page POST and don't
        // need the outbox machinery - they're per-boundary UI state
        // edits with no outgoing messages. A plain SQLite UPSERT is
        // enough; atomicity with messaging only matters at submit.
        await EnsureTable(ct);

        var json = JsonSerializer.Serialize(slice, sliceType);

        await using var connection = new SqliteConnection(options.ConnectionString);
        await connection.OpenAsync(ct);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $@"
            INSERT INTO {options.TableName} (WorkflowId, SliceKey, SliceJson, UpdatedAt)
            VALUES ($id, $key, $json, $now)
            ON CONFLICT(WorkflowId, SliceKey) DO UPDATE SET
                SliceJson = excluded.SliceJson,
                UpdatedAt = excluded.UpdatedAt;";
        cmd.Parameters.AddWithValue("$id", workflowId.ToString());
        cmd.Parameters.AddWithValue("$key", sliceKey);
        cmd.Parameters.AddWithValue("$json", json);
        cmd.Parameters.AddWithValue("$now", DateTimeOffset.UtcNow.ToString("O"));
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task Submit(Guid workflowId, IReadOnlyList<object> commands, CancellationToken ct)
    {
        // The atomic envelope. TransactionalSession's commit writes
        // the outbox row in the same SQLite file the slice rows live
        // in. The processor endpoint dispatches the queued commands
        // after commit; if the gateway crashes mid-commit nothing is
        // sent and nothing is written.
        await session.Open(new SqliteOpenSessionOptions(), ct);

        // Mark the workflow as submitted so subsequent reads can tell
        // the user "this checkout already went through" if they hit
        // back. The write goes through the session's connection so it
        // commits with the outbox row.
        var sqlite = session.SynchronizedStorageSession.SqliteSession();
        await using (var cmd = sqlite.CreateCommand())
        {
            cmd.CommandText = $@"
                INSERT INTO {options.TableName} (WorkflowId, SliceKey, SliceJson, UpdatedAt)
                VALUES ($id, '$submitted', '{{}}', $now)
                ON CONFLICT(WorkflowId, SliceKey) DO UPDATE SET
                    UpdatedAt = excluded.UpdatedAt;";
            cmd.Parameters.AddWithValue("$id", workflowId.ToString());
            cmd.Parameters.AddWithValue("$now", DateTimeOffset.UtcNow.ToString("O"));
            await cmd.ExecuteNonQueryAsync(ct);
        }

        foreach (var command in commands)
        {
            await session.Send(command, ct);
        }

        await session.Commit(ct);
    }

    public async Task<int> DeleteInactive(TimeSpan inactivity, CancellationToken ct)
    {
        await EnsureTable(ct);

        var cutoff = DateTimeOffset.UtcNow.Subtract(inactivity).ToString("O");

        await using var connection = new SqliteConnection(options.ConnectionString);
        await connection.OpenAsync(ct);

        await using var cmd = connection.CreateCommand();
        // Group by WorkflowId so a single fresh slice keeps the whole
        // workflow alive; only workflows whose most recent write is
        // older than the cutoff are dropped.
        cmd.CommandText = $@"
            DELETE FROM {options.TableName}
            WHERE WorkflowId IN (
                SELECT WorkflowId FROM {options.TableName}
                GROUP BY WorkflowId
                HAVING MAX(UpdatedAt) < $cutoff
            );";
        cmd.Parameters.AddWithValue("$cutoff", cutoff);
        return await cmd.ExecuteNonQueryAsync(ct);
    }

    async Task EnsureTable(CancellationToken ct)
    {
        if (tableEnsured) return;
        await ensureLock.WaitAsync(ct);
        try
        {
            if (tableEnsured) return;

            await using var connection = new SqliteConnection(options.ConnectionString);
            await connection.OpenAsync(ct);

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {options.TableName} (
                    WorkflowId TEXT NOT NULL,
                    SliceKey   TEXT NOT NULL,
                    SliceJson  TEXT NOT NULL,
                    UpdatedAt  TEXT NOT NULL,
                    PRIMARY KEY (WorkflowId, SliceKey)
                );";
            await cmd.ExecuteNonQueryAsync(ct);

            tableEnsured = true;
        }
        finally
        {
            ensureLock.Release();
        }
    }
}
