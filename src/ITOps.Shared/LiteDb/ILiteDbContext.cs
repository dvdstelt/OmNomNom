using LiteDB;

namespace ITOps.Shared;

public interface ILiteDbContext
{
    LiteDatabase Database { get; }
}