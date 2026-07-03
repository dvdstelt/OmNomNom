using System.Reflection;
using Messaging.Persistence.Sqlite;
using NServiceBusContrib.HealthCheck;
using NServiceBusContrib.WarmUp;

namespace ITOps.Shared.EndpointConfiguration;

public static class EndpointConfigurationExtensions
{
    public static NServiceBus.EndpointConfiguration Configure(
        this NServiceBus.EndpointConfiguration endpointConfiguration,
        string? sqliteConnectionString = null,
        Action<PersistenceExtensions<SqlitePersistence>>? configurePersistence = null,
        Action<RoutingSettings<LearningTransport>>? configureRouting = null)
    {
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

        // Outbox in NServiceBus 10 requires ReceiveOnly so the transport
        // doesn't try to wrap message processing in its own transaction.
        // Harmless for endpoints that don't enable the outbox.
        var learningTransport = new LearningTransport
        {
            TransportTransactionMode = TransportTransactionMode.ReceiveOnly
        };

        // In containers the gateway (send-only) and the AllInOne endpoint
        // host run as separate processes and must share one transport
        // folder. OMNOMNOM_TRANSPORT_DIR pins it; unset, NServiceBus keeps
        // its solution-relative default for local dev.
        var transportDir = Environment.GetEnvironmentVariable("OMNOMNOM_TRANSPORT_DIR");
        if (!string.IsNullOrWhiteSpace(transportDir))
        {
            learningTransport.StorageDirectory = transportDir;
        }

        var routing = endpointConfiguration.UseTransport(learningTransport);

        if (sqliteConnectionString != null)
        {
            var persistence = endpointConfiguration.UsePersistence<SqlitePersistence>();
            persistence.ConnectionString(sqliteConnectionString);
            configurePersistence?.Invoke(persistence);
            endpointConfiguration.EnableOutbox();
        }

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages.Commands"));
        conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages.Events"));

        endpointConfiguration.EnableInstallers();

        // Track readiness so the AllInOne host's /health can report each endpoint. No warm-up
        // actions are configured, so this does not delay the pump; it just records Starting -> Ready.
        endpointConfiguration.WarmUp();

        // Liveness: each endpoint heartbeats its own queue so a hung endpoint in a multi-endpoint
        // host is detected and reported via /health/live. Only actually sends when a host consumes
        // liveness (the AllInOne registers the checks); a no-op for standalone endpoint hosts.
        endpointConfiguration.EnableLivenessHeartbeat(heartbeat =>
        {
            heartbeat.Interval(TimeSpan.FromSeconds(10));
            heartbeat.StaleAfter(TimeSpan.FromSeconds(30));
        });

        configureRouting?.Invoke(routing);
        ApplyDiscoveredRoutingConfigurators(routing);

        return endpointConfiguration;
    }

    static void ApplyDiscoveredRoutingConfigurators(RoutingSettings<LearningTransport> routing)
    {
        var configurators = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IConfigureEndpointRouting).IsAssignableFrom(t))
            .Select(t => (IConfigureEndpointRouting)Activator.CreateInstance(t)!);

        foreach (var configurator in configurators)
        {
            configurator.ConfigureRouting(routing);
        }
    }

    static Type[] GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).ToArray()!;
        }
    }
}
