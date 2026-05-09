using Catalog.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Workflow;

// Marker slice that signals the workflow is ready to be finalized.
// Written by WorkflowSubmitHandler immediately before
// IWorkflowSubmitter.Submit so the CompleteOrder command becomes
// part of the atomic dispatch bundle. The slice carries no payload
// of its own - the workflow id is enough to build the command.
public sealed record CompleteOrderSlice;

public class CompleteOrderWorkflowSlice : WorkflowSlice<CompleteOrderSlice>
{
    public const string Key = "Catalog.CompleteOrder";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, CompleteOrderSlice slice) =>
        new CompleteOrder { OrderId = orderId };
}
