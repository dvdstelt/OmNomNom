namespace WorkflowComposer;

internal sealed class WorkflowSubmitter(
    IWorkflowStore store,
    IEnumerable<IWorkflowSlice> slices) : IWorkflowSubmitter
{
    public async Task Submit(Guid workflowId, CancellationToken ct = default)
    {
        var loaded = new List<(IWorkflowSlice slice, object value)>();
        var errors = new Dictionary<string, IReadOnlyList<string>>();

        foreach (var slice in slices.OrderBy(s => s.SubmitOrder))
        {
            var value = await store.ReadSlice(workflowId, slice.SliceKey, slice.SliceType, ct);
            if (value is null) continue;

            var sliceErrors = slice.Validate(value);
            if (sliceErrors.Count > 0)
                errors[slice.SliceKey] = sliceErrors;
            else
                loaded.Add((slice, value));
        }

        if (errors.Count > 0)
            throw new WorkflowValidationException(workflowId, errors);

        var commands = new List<object>();
        foreach (var (slice, value) in loaded)
        {
            var command = slice.BuildSubmitCommand(workflowId, value);
            if (command is not null) commands.Add(command);
        }

        await store.Submit(workflowId, commands, ct);
    }
}
