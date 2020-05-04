using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups
{
	/// <summary>
	/// Zajišťuje distribuovanou invalidaci lookup dat.
	/// </summary>
	public interface IDistributedLookupDataInvalidationService
	{
		/// <summary>
		/// Invaliduje data v úložišti s daným klíčem.
		/// </summary>
		public void Invalidate(string storageKey);
	}
}
