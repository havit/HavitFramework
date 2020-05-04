namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups
{
	/// <summary>
	/// Nedělá nic.
	/// </summary>
	public class NullDistributedLookupDataInvalidationService : IDistributedLookupDataInvalidationService
	{
		/// <summary>
		/// Nedělá nic.
		/// </summary>
		public void Invalidate(string storageKey)
		{
			// NOOP
		}
	}
}
