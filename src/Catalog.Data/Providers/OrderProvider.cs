using ITOps.Shared.Integration;
using NServiceBus.Logging;

namespace Catalog.Data.Providers;

public class OrderProvider : IProvideOrderData
{
    static ILog log = LogManager.GetLogger<OrderProvider>();

    public Order GetOrderInfo(Guid orderId)
    {
        log.Info("Yep, called!");

        return null!;
    }
}