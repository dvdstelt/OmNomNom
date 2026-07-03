using Marketing.Endpoint.Messages.Commands;
using NServiceBus;

namespace Marketing.Endpoint.Handlers;

// CHAOS / TEST ONLY. Never returns, so it permanently holds a message-processing
// concurrency slot. Spray enough of these (via the chaos-gated /debug/hang
// endpoint on the AllInOne host) to occupy every slot and the pump stops picking
// up anything else - including its own liveness heartbeat, which then goes stale
// and flips /health/live to unhealthy. The endpoint is "hung but alive": the host
// process is fine, only this endpoint stops processing. That is exactly the
// failure the per-endpoint heartbeat exists to catch. Recovery is a container
// restart (autoheal), which is the point of the test.
//
// The handler is always registered (it is discovered by Handlers.Marketing.AddAll),
// but it is inert unless a HangMarketing message arrives, and the only sender is
// the /debug/hang endpoint, which exists only when OMNOMNOM_CHAOS is set.
[Handler]
public class HangMarketingHandler
{
    // No cancellation token on purpose: a real hang doesn't politely unwind on
    // shutdown. Recovery is a restart.
    public Task Handle(HangMarketing message, IMessageHandlerContext context)
        => Task.Delay(Timeout.InfiniteTimeSpan, context.CancellationToken);
}
