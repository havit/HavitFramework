using System.Data;
using System.Data.Common;
using Havit.Data.TransientErrorHandling;

namespace Havit.Data.Extensions;

/// <summary>
/// Extension methody k DbCommend.
/// </summary>
internal static class DbCommandExt
{
	/// <summary>
	/// Volá ExecuteNonQuery. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
	/// </summary>
	public static int ExecuteNonQueryWithRetry(this DbCommand command)
	{
		return TransientErrorHandler.ExecuteAction(command.ExecuteNonQuery, command.CanRetryCommand);
	}

	/// <summary>
	/// Volá ExecuteScalar. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
	/// </summary>
	public static object ExecuteScalarWithRetry(this DbCommand command)
	{
		return TransientErrorHandler.ExecuteAction(command.ExecuteScalar, command.CanRetryCommand);
	}

	/// <summary>
	/// Volá ExecuteReader. V případě neúspěchu z důvodu transientní chyby pokus opakuje.
	/// </summary>
	public static DbDataReader ExecuteReaderWithRetry(this DbCommand command, CommandBehavior behavior)
	{
		return TransientErrorHandler.ExecuteAction(() => command.ExecuteReader(behavior), command.CanRetryCommand);
	}

	/// <summary>
	/// Vrací true, pokud je možné command zopakovat.
	/// To je možné tehdy, pokud není uzavřeno spojení.
	/// </summary>
	private static bool CanRetryCommand(this DbCommand command)
	{
		return (command.Connection.State == ConnectionState.Open); // musí být stále otevřené spojení
	}
}
