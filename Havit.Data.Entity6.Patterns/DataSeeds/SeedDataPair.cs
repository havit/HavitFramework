namespace Havit.Data.Entity.Patterns.DataSeeds
{
	internal class SeedDataPair<TEntity>
	{
		public TEntity SeedEntity { get; set; }
		public TEntity DbEntity { get; set; }
		public bool IsNew { get; set; }
	}
}