using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes
{
	/// <summary>
	/// Výrazy pro opakované použití s DefaultValueSqlAttribute.
	/// </summary>
	public static class DefaultValueSql
	{
		/// <summary>
		/// Výraz pro GETDATE().
		/// </summary>
		public const string GetDate = "GETDATE()";
	}
}
