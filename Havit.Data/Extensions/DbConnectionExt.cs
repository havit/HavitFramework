using System.Data.Common;
using Havit.Data.TransientErrorHandling;

namespace Havit.Data.Extensions;

/// <summary>
/// Extension methody k DbConnection.
/// </summary>
internal static class DbConnectionExt
{
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
}
