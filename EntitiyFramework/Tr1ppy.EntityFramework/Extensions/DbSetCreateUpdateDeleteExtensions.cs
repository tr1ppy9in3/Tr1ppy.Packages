using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Tr1ppy.EntityFramework.Extensions;

public static class DbSetCreateUpdateDeleteExtensions
{
    public static async Task AddAndSaveAsync<TEntity>
    (
        this DbSet<TEntity> dbSet,
        TEntity entity,
        DbContext context,
        ILogger? logger = null
    ) 
        where TEntity : class
    {
        logger?.LogTrace
        (
             message: "Starting AddAndSaveAsync for entity of type {EntityType}",
             args: [typeof(TEntity).Name]
        );

        try
        {
            dbSet.Add(entity);
            logger?.LogTrace
            (
                message: "Entity added: {Entity}",
                args: [entity]
            );

            await context.SaveChangesAsync();
            logger?.LogInformation
            (
                message: "Successfully added entity of type {EntityType}",
                args: [typeof(TEntity).Name]
            );
        }
        catch (Exception ex)
        {
            logger?.LogError
            (
                exception: ex,
                message: "Failed to add entity of type {EntityType}",
                args: [typeof(TEntity).Name]
            );
            throw;
        }
    }
}
