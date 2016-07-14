using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.TransientErrorHandling;

namespace Havit.Data.Extensions
{
	/// <summary>
	/// Extension methody k DbConnection.
	/// </summary>
	internal static class DbConnectionExt
	{
		#region OpenWithRetry
		/// <summary>
		/// Otevírá databázové spojení. V případě neúspěchu z důvodu transientní chyby pokus o otevření opakuje.
		/// </summary>
		public static void OpenWithRetry(this DbConnection connection)
		{
			TransientErrorHandler.ExecuteAction<object>(
				() =>
				{
					connection.Open();
					return null;
				},
				() => true // vždy můžeme opakovat
			);
		}
		#endregion
	}
}
