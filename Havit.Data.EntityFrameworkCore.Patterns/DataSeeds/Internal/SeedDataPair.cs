namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;

internal class SeedDataPair<TEntity>
	where TEntity : class
{
	public TEntity SeedEntity { get; set; }
	public TEntity DbEntity { get; set; }
	public bool IsNew { get; set; }
}
