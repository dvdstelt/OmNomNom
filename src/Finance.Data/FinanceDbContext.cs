using ITOps.Shared;

namespace Finance.Data;

public class FinanceDbContext(LiteDbOptions options) : LiteDbContext(options);