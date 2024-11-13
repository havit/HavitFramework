namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;

public partial class DbDataLoader
{
	private ICollection<TEntity> LoadPropertyInternal_EntitiesToCollectionOptimized<TEntity>(IEnumerable<TEntity> entities)
	{
		if (entities is ICollection<TEntity> entitiesCollection)
		{
			return entitiesCollection;
		}
		else
		{
			// Jde o odloženou enumeraci tak, aby poslední Load/ThenLoad v řetězci DataLoader.Load(...).ThenLoad()
			// nemusel alokovat pole (a filtrovat Distinct, atp.).
			// Viz metoda LoadCollectionPropertyInternal_GetResult.
			return entities.ToArray();
		}
	}
}
