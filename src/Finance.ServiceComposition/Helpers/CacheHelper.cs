using System.Text;
using System.Text.Json;
using Finance.Data.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Finance.ServiceComposition.Helpers;

public class CacheHelper(IDistributedCache distributedCache)
{
    const string OrderSessionKeyName = "FinanceOrder";

    public async Task<Order> GetOrder(Guid orderId)
    {
        var shortOrderId = orderId.ToString()[..8];
        var serializedOrder = await distributedCache.GetAsync($"{OrderSessionKeyName}{shortOrderId}");
        var order = serializedOrder == null ? new() { OrderId = orderId } : JsonSerializer.Deserialize<Order>(serializedOrder);

        return order!;
    }

    public async Task StoreOrder(Order order)
    {
        var shortOrderId = order.OrderId.ToString()[..8];

        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        var serializedOrder = JsonSerializer.Serialize(order);
        await distributedCache.SetAsync($"{OrderSessionKeyName}{shortOrderId}", Encoding.UTF8.GetBytes(serializedOrder), options);
    }

}