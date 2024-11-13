namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

internal static class DbDataLoaderHelpers
{
	public static void CheckEntityIsTracked<TEntity>(TEntity entity, IDbContext dbContext)
	{
		if (dbContext.GetEntry(entity, suppressDetectChanges: true).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
		{
			throw new InvalidOperationException($"DbDataLoader can be used only for objects tracked by a change tracker. Entity '{entity}' is not tracked.");
		}
	}

	public static IEnumerable<TEntity> WithTrackedEntitiesCheck<TEntity>(this IEnumerable<TEntity> source, IDbContext dbContext)
	{
		foreach (TEntity entity in source)
		{
			CheckEntityIsTracked(entity, dbContext);
			yield return entity;
		}
	}
}
