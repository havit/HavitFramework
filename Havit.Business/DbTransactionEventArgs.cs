using System.Data.Common;

namespace Havit.Business;

/// <summary>
/// Argument nesoucí instanci databázové transakce.
/// </summary>
public class DbTransactionEventArgs : EventArgs
{
	/// <summary>
	/// Transakce.
	/// Pro OnBeforeSave a OnAfterSave nemůže být v případě ActiveRecordBusinessObjectBase null, v případě holého BusinessObjectBase ano.
	/// </summary>
	public DbTransaction Transaction
	{
		get { return _transaction; }
	}
	private readonly DbTransaction _transaction;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbTransactionEventArgs(DbTransaction transaction)
	{
		this._transaction = transaction;
	}
}
