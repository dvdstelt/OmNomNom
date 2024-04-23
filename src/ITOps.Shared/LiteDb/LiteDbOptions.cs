using System.Collections.Concurrent;
using LiteDB;

namespace ITOps.Shared;

public class LiteDbOptions(string databaseName, Action<LiteDatabase> databaseInitializer)
{
    public string DatabaseLocation { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = databaseName;
    public Action<LiteDatabase> DatabaseInitializer { get; set; } = databaseInitializer;
}
