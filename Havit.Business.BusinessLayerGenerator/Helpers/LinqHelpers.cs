using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	/// <summary>
	/// Metody pro usnadnění práce se třídami, které nepodporují LINQ (resp. neimplementují IEnumerable), ačkoliv by klidně.
	/// </summary>
	public static class LinqHelpers
	{
		/// <summary>
		/// AsEnumerable pro ForeignKeyCollection.
		/// </summary>
		public static IEnumerable<ForeignKey> AsEnumerable(this ForeignKeyCollection foreignKeyCollection)
		{
			if (foreignKeyCollection == null)
			{
				throw new ArgumentNullException("foreignKeyCollection");
			}

			foreach (ForeignKey fk in foreignKeyCollection)
			{
				yield return fk;
			}
		}
	}
}
