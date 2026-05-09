# Getting started

Minimum wiring for a single-slice workflow with the SQLite backend.

## 1. Reference the packages

In the host that owns the gateway endpoint:

```xml
<ProjectReference Include="..\WorkflowComposer\WorkflowComposer.csproj" />
<ProjectReference Include="..\WorkflowComposer.Sqlite\WorkflowComposer.Sqlite.csproj" />
```

In each service boundary that contributes a slice:

```xml
<ProjectReference Include="..\WorkflowComposer\WorkflowComposer.csproj" />
```

Boundaries don't need the SQLite backend - they only see the abstractions.

## 2. Define a slice

```csharp
public record CartSlice(IReadOnlyList<CartItem> Items);

public record CartItem(Guid ProductId, int Quantity);

public class CartWorkflowSlice : WorkflowSlice<CartSlice>
{
    public override string SliceKey => "Catalog.Cart";

    protected override object BuildSubmitCommand(Guid orderId, CartSlice slice) =>
        new SubmitOrderItems
        {
            OrderId = orderId,
            Items = slice.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };
}
```

## 3. Register the framework + backend

In the gateway's `Program.cs`, after building the `EndpointConfiguration`:

```csharp
var endpointConfiguration = new EndpointConfiguration("CompositionGateway");
endpointConfiguration.SendOnly();
// ... transport, conventions, routing ...

builder.Services.AddWorkflowComposer(workflow =>
{
    workflow.UseSqliteStore(endpointConfiguration, new SqliteWorkflowStoreOptions
    {
        ConnectionString = "Data Source=checkout.db",
        ProcessorEndpoint = "Checkout"
    });

    workflow.RegisterSlicesFromAssembliesOf(typeof(CartWorkflowSlice));
});

builder.UseNServiceBus(endpointConfiguration);
```

`UseSqliteStore` configures the NServiceBus persister, enables outbox + transactional session on the gateway endpoint, and registers `IWorkflowStore`. `RegisterSlicesFromAssembliesOf` discovers every `IWorkflowSlice` in the named assemblies and registers them.

The `ProcessorEndpoint` (here `"Checkout"`) is a separate NServiceBus endpoint that handles the control message TransactionalSession sends on commit and dispatches the queued outbox messages. It can be a tiny endpoint with no handlers - it just needs the same SQLite persister, outbox enabled, and `EnableTransactionalSession()` so it knows how to receive the control message.

## 4. Use the store from per-page handlers

```csharp
public class ShoppingCartAddItemHandler(IWorkflowStore store) : ICompositionRequestsHandler
{
    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;
        var orderId = ParseOrderId(request);
        var input = await request.Bind<...>();

        var cart = await store.Read<CartSlice>(orderId, "Catalog.Cart", ct)
                   ?? new CartSlice([]);
        var updated = Apply(cart, input);
        await store.Write(orderId, "Catalog.Cart", updated, ct);
    }
}
```

Reads come from the same store, so read-your-writes is automatic.

## 5. Submit

```csharp
public class WorkflowSubmitHandler(IWorkflowSubmitter submitter) : ICompositionRequestsHandler
{
    [HttpPost("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderId = ParseOrderId(request);
        await submitter.Submit(orderId, request.HttpContext.RequestAborted);
    }
}
```

That's it. The framework reads each registered slice, calls `BuildSubmitCommand`, and hands the bag to the store. The SQLite store opens a TransactionalSession, sends each command, marks the workflow submitted, and commits in one SQLite transaction.

## 6. Set up the processor endpoint

The processor endpoint is a regular NServiceBus host pointed at the same SQLite file:

```csharp
var endpointConfiguration = new EndpointConfiguration("Checkout");
var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
persistence.ConnectionString("Data Source=checkout.db");
persistence.EnableTransactionalSession();
endpointConfiguration.EnableOutbox();
// ... transport, etc ...
```

It has no message handlers of its own; its job is to receive the control message TransactionalSession sends on each commit and trigger outbox dispatch.
