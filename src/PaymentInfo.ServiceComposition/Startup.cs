using ITOps.Shared;
using Microsoft.Extensions.DependencyInjection;
using PaymentInfo.Data;
using PaymentInfo.Data.Migrations;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddSingleton<PaymentInfoDbContext>(provider =>
        {
            var liteDbOptions = new LiteDbOptions("paymentinfo", DatabaseInitializer.Initialize);
            return new PaymentInfoDbContext(liteDbOptions);
        });
    }
}