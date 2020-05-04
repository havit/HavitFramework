namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups
{
	internal class EntityLookupPair<TEntityKey, TLookupKey>
	{
		public TEntityKey EntityKey { get; set; }
		public TLookupKey LookupKey { get; set; }
	}
}
