namespace ITOps.Shared.EndpointConfiguration;

using System;
using Messaging.Persistence.Sqlite;
using NServiceBus;
using NServiceBus.Persistence;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration Configure(
        this EndpointConfiguration endpointConfiguration,
        string sqliteConnectionString = null,
        Action<PersistenceExtensions<SqlitePersistence>> configurePersistence = null,
        Action<RoutingSettings<LearningTransport>> configureRouting = null)
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

        configureRouting?.Invoke(routing);

        return endpointConfiguration;
    }
}
