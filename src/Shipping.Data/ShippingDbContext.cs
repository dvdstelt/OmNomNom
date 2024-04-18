using ITOps.Shared;

namespace Shipping.Data;

public class ShippingDbContext(LiteDbOptions options) : LiteDbContext(options);