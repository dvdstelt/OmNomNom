using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using OmNomNom.Website.ViewModelComposition;

namespace Temporary.Caching;

public class CacheHelper(IDistributedCache distributedCache)
{
    const string UserCartSessionKeyName = "UserCart";
    readonly IDistributedCache distributedCache = distributedCache;

    public async Task<UserCart> GetCart(Guid orderId)
    {
        var shortOrderId = orderId.ToString()[..8];
        var serializedCart = await distributedCache.GetAsync($"{UserCartSessionKeyName}{shortOrderId}");
        var cart = serializedCart == null ? new() { OrderId = orderId } : JsonSerializer.Deserialize<UserCart>(serializedCart);

        return cart;
    }

    public async Task StoreCart(UserCart cart)
    {
        var shortOrderId = cart.OrderId.ToString()[..8];

        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));

        var serializedCart = JsonSerializer.Serialize(cart);
        await distributedCache.SetAsync($"{UserCartSessionKeyName}{shortOrderId}", Encoding.UTF8.GetBytes(serializedCart), options);
    }
}