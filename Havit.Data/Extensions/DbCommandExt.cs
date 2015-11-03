using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.TransientErrorHandling;

namespace Havit.Data.Extensions
{
	/// <summary>
	/// Extension methody k DbCommend.
	/// </summary>
	internal static class DbCommandExt
	{
		#region ExecuteNonQueryWithRetry
		/// <summary>
		/// Volá ExecuteNonQuery. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
		/// </summary>
		public static int ExecuteNonQueryWithRetry(this DbCommand command)
		{
			return TransientErrorHandler.ExecuteAction(command.ExecuteNonQuery);
		}
		#endregion

		#region ExecuteScalarWithRetry
		/// <summary>
		/// Volá ExecuteScalar. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
		/// </summary>
		public static object ExecuteScalarWithRetry(this DbCommand command)
		{
			return TransientErrorHandler.ExecuteAction(command.ExecuteScalar);
		}
		#endregion

		#region ExecuteReaderWithRetry
		/// <summary>
		/// Volá ExecuteReader. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
		/// </summary>
		public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, CommandBehavior behavior)
		{
			return TransientErrorHandler.ExecuteAction(() => command.ExecuteReader(behavior));
		}
		#endregion
	}
}
