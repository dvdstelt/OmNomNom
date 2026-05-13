using NServiceBus;

namespace ITOps.Shared.EndpointConfiguration;

public interface IConfigureEndpointRouting
{
    void ConfigureRouting(RoutingSettings<LearningTransport> routing);
}
