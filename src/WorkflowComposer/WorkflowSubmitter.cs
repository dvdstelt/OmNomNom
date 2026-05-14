namespace WorkflowComposer;

internal sealed class WorkflowSubmitter(
    IWorkflowStore store,
    IEnumerable<IWorkflowSlice> slices) : IWorkflowSubmitter
{
    public async Task Submit(Guid workflowId, CancellationToken ct = default)
    {
        var commands = new List<object>();

        foreach (var slice in slices.OrderBy(s => s.SubmitOrder))
        {
            var value = await store.ReadSlice(workflowId, slice.SliceKey, slice.SliceType, ct);
            if (value is null) continue;

            var command = slice.BuildSubmitCommand(workflowId, value);
            if (command is null) continue;

            commands.Add(command);
        }

        await store.Submit(workflowId, commands, ct);
    }
}
