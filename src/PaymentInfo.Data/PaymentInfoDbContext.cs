using ITOps.Shared;

namespace PaymentInfo.Data;

public class PaymentInfoDbContext(LiteDbOptions options) : LiteDbContext(options);