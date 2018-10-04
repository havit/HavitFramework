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
			return TransientErrorHandler.ExecuteAction(command.ExecuteNonQuery, command.CanRetryCommand);
		}
		#endregion

		#region ExecuteScalarWithRetry
		/// <summary>
		/// Volá ExecuteScalar. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
		/// </summary>
		public static object ExecuteScalarWithRetry(this DbCommand command)
		{
			return TransientErrorHandler.ExecuteAction(command.ExecuteScalar, command.CanRetryCommand);
		}
		#endregion

		#region ExecuteReaderWithRetry
		/// <summary>
		/// Volá ExecuteReader. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
		/// </summary>
		public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, CommandBehavior behavior)
		{
			return TransientErrorHandler.ExecuteAction(() => command.ExecuteReader(behavior), command.CanRetryCommand);
		}
		#endregion

		#region CanRetryCommand
		/// <summary>
		/// Vrací true, pokud je možné command zopakovat.
		/// To je možné tehdy, pokud není uzavřeno spojení.
		/// </summary>
		private static bool CanRetryCommand(this DbCommand command)
		{
			return (command.Connection.State == ConnectionState.Open); // musí být stále otevřené spojení
		}
		#endregion

	}
}
