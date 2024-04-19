using ITOps.Shared;

namespace Marketing.Data;

public class MarketingDbContext(LiteDbOptions options) : LiteDbContext(options);