using System;
using System.Data.Common;

namespace Havit.Data.Entity.Glimpse.DbCommandInterception;

/// <summary>
/// Informace o provedeném DbCommandu.
/// </summary>
internal class DbCommandLogItem
{
	/// <summary>
	/// Operace (ExecuteScalar, ExecuteReader, ExecuteNonQuery).
	/// </summary>
	public string Operation { get; set; }

	/// <summary>
	/// Provedený databázový dotaz.
	/// </summary>
	public DbCommand Command { get; set; }

	/// <summary>
	/// Indikuje, zda byl dotaz proveden asynchronně.
	/// </summary>
	public bool IsAsync { get; set; }

	/// <summary>
	/// Indikuje, k jeké došlo výjimce.
	/// </summary>
	public Exception Exception { get; set; }

	/// <summary>
	/// Výsledek dotazu. Pro ExecuteScalar je zde hodnota, která je výsledkem. Pro ExecuteDataReader je zde instance DbDataReaderResult, která udává počet záznamů v DbDataReaderu.
	/// </summary>
	public object Result { get; set; }

	/// <summary>
	/// Doba trvání dotazu.
	/// </summary>
	public long DurationTicks { get; set; }
}
