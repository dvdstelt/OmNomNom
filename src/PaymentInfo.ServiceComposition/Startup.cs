using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentInfo.Data;
using PaymentInfo.Endpoint.Messages.Commands;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization, IConfigureEndpointRouting
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<PaymentInfoDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("paymentinfo")));
    }

    public void ConfigureRouting(RoutingSettings<LearningTransport> routing)
    {
        routing.RouteToEndpoint(typeof(SubmitPaymentInfo).Assembly, "PaymentInfo");
    }
}
