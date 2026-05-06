namespace ITOps.Shared.Sqlite;

// Shared connection-string builder for the per-service SQLite files.
// Walks up from the running assembly until it hits the .sln (so all
// services land in the same `<repo>/src/.db/` folder), or stops at any
// directory that already contains a `.db/` folder. Each service passes
// its own short name (catalog, finance, etc.) so the files stay
// one-per-service.
public static class SqliteStorage
{
    const string DefaultDatabaseDirectory = ".db";

    public static string GetConnectionString(string databaseName)
    {
        var storagePath = FindStoragePath();
        Directory.CreateDirectory(storagePath);
        var path = Path.Combine(storagePath, databaseName.ToLowerInvariant() + ".db");
        return $"Data Source={path}";
    }

    static string FindStoragePath()
    {
        var directory = AppDomain.CurrentDomain.BaseDirectory;

        while (true)
        {
            if (Directory.EnumerateFiles(directory).Any(file => file.EndsWith(".sln")))
            {
                return Path.Combine(directory, DefaultDatabaseDirectory);
            }

            var databaseDirectory = Path.Combine(directory, DefaultDatabaseDirectory);
            if (Directory.Exists(databaseDirectory))
            {
                return databaseDirectory;
            }

            var parent = Directory.GetParent(directory);

            if (parent == null)
            {
                throw new Exception(
                    $"Unable to determine the storage directory path for the database due to the absence of a solution file. Please create a '{DefaultDatabaseDirectory}' directory in one of this project's parent directories.");
            }

            directory = parent.FullName;
        }
    }
}
