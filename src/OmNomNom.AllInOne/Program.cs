// ============================================================================
//  OmNomNom.AllInOne - single-process host for every NServiceBus endpoint
// ============================================================================
//
//  This project is the "Compound B" demo: every backend message endpoint
//  (Catalog, Finance, Marketing, Shipping, PaymentInfo, Checkout) is hosted
//  inside ONE .NET process. The service boundaries themselves are unchanged -
//  same code, same handlers, same SQLite files. Only the deployment topology
//  is different.
//
//  *** DO NOT RUN THIS ALONGSIDE THE INDIVIDUAL ENDPOINTS ***
//
//  Each NServiceBus endpoint binds to a queue folder under .learningtransport/
//  by its endpoint name. If you run this process AND Catalog.Endpoint at the
//  same time, two processes will both poll the "Catalog" folder, race over
//  the same message files, and silently double-process - one handler may see
//  the message, the other may see a half-deleted body file and log an error.
//  Order state will become inconsistent.
//
//  Pick exactly one of:
//    - the existing Rider compound "Website + Endpoints" (every endpoint in
//      its own process), OR
//    - the new Rider compound "Website + AllInOne" (this process), but NOT
//      the individual endpoint launchers.
//
//  CompositionGateway and OmNomNom.BackOffice stay as their own processes in
//  both compounds: they are ASP.NET hosts (HTTP + Razor) more than message
//  endpoints, and the talk's "boundary != process" point is about the
//  business-message endpoints. Folding them in would conflate concerns.
//
//  Powered by NServiceBus 10.2's AddNServiceBusEndpoint (PR #7633): each
//  endpoint configuration gets its own keyed DI scope, distinct LearningTransport
//  instance, and slot-scoped logging. Assembly scanning is disabled in every
//  AddXEndpoint() extension so the configs don't pick up each other's [Handler]
//  types from the shared load context. Each endpoint also registers an
//  IHostedLifecycleService database seeder, so EnsureCreatedAsync + seed runs
//  in StartingAsync before the message pumps come online - no explicit
//  initializer calls needed below.
// ============================================================================

//  This host also exposes container health probes over HTTP (hence WebApplication):
//    /health/ready - every endpoint has completed warm-up (readiness)
//    /health/live  - every endpoint is alive (liveness); a warming-up endpoint is
//                    alive-but-not-ready. Backed by NServiceBusContrib.HealthCheck,
//                    which aggregates all endpoints via the shared status registry.

using Catalog.Endpoint;
using Checkout.Endpoint;
using Finance.Endpoint;
using Marketing.Endpoint;
using Marketing.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NServiceBus;
using NServiceBusContrib.HealthCheck;
using PaymentInfo.Endpoint;
using Shipping.Endpoint;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddCatalogEndpoint();
services.AddFinanceEndpoint();
services.AddMarketingEndpoint();
services.AddShippingEndpoint();
services.AddPaymentInfoEndpoint();
services.AddCheckoutEndpoint();

// Readiness + liveness health checks aggregating every endpoint in this process.
services.AddHealthChecks()
    .AddNServiceBusReadiness()
    .AddNServiceBusLiveness();

var app = builder.Build();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});

// CHAOS / TEST ONLY. Enabled by OMNOMNOM_CHAOS=1. Sprays blocking messages onto a
// target endpoint's own queue to saturate its message pump, so its liveness
// heartbeat goes stale and /health/live flips to unhealthy - letting us verify
// the heartbeat detection (and, in a container, the autoheal restart). Only
// Marketing carries the blocking handler today; add an equivalent to another
// endpoint's assembly to make it hangable too.
if (Environment.GetEnvironmentVariable("OMNOMNOM_CHAOS") is "1" or "true")
{
    app.MapPost("/debug/hang/", async (int? count, IServiceProvider sp) =>
    {
        // Each endpoint's IMessageSession is registered keyed by its endpoint name.
        var session = sp.GetRequiredKeyedService<IMessageSession>("Marketing");
        var spray = count ?? 200;
        for (var i = 0; i < spray; i++)
        {
            await session.SendLocal(new HangMarketing());
        }

        return Results.Ok(
            $"Sprayed {spray} HangMarketing messages onto Marketing. Its pump should saturate " +
            "and /health/live report unhealthy within the heartbeat StaleAfter window.");
    });
}

await app.RunAsync();
