using Microsoft.EntityFrameworkCore;

namespace Tr1ppy.EntityFramework;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder ApplyProvider
    (
        this DbContextOptionsBuilder optionsBuilder, 
        DbProvider dbProvider,
        string connectionString
    )
    {
        switch (dbProvider)
        {
            case DbProvider.Npgsql:
                optionsBuilder.UseNpgsql(connectionString);
                break;

            //case DbProvider.Sqlite:
            //    optionsBuilder.UseSqlite(connectionString);
            //    break;

            default:
                throw new NotSupportedException($"Провайдер {dbProvider} не поддерживается.");
        }

        return optionsBuilder;
    }
}
