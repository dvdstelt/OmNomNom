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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services
    .AddCatalogEndpoint()
    .AddFinanceEndpoint()
    .AddMarketingEndpoint()
    .AddShippingEndpoint()
    .AddPaymentInfoEndpoint()
    .AddCheckoutEndpoint();

var host = hostBuilder.Build();
Console.Title = "OmNomNom AllInOne";
await host.RunAsync();
