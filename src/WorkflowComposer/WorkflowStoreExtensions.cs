namespace WorkflowComposer;

public static class WorkflowStoreExtensions
{
    public static async Task<TSlice?> Read<TSlice>(
        this IWorkflowStore store,
        Guid workflowId,
        string sliceKey,
        CancellationToken ct = default)
        where TSlice : class
    {
        var value = await store.ReadSlice(workflowId, sliceKey, typeof(TSlice), ct);
        return (TSlice?)value;
    }

    public static Task Write<TSlice>(
        this IWorkflowStore store,
        Guid workflowId,
        string sliceKey,
        TSlice slice,
        CancellationToken ct = default)
        where TSlice : class
    {
        ArgumentNullException.ThrowIfNull(slice);
        return store.WriteSlice(workflowId, sliceKey, slice, typeof(TSlice), ct);
    }
}
