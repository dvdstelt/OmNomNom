﻿using ITOps.Shared;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Migrations;
using Shipping.ServiceComposition.Helpers;

namespace Shipping.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddSingleton<ShippingDbContext>(provider =>
        {
            var dbOptions = new LiteDbOptions("shipping", DatabaseInitializer.Initialize);
            return new ShippingDbContext(dbOptions);
        });
        options.Services.AddSingleton<CacheHelper>();
    }
}