using System.Data.Common;

namespace Havit.Data;

/// <summary>
/// Reprezentuje metodu, která vykonává jednotlivé kroky transakce.
/// </summary>
/// <param name="transaction">transakce, v rámci které mají být jednotlivé kroky vykonány</param>	
public delegate void DbTransactionDelegate(DbTransaction transaction);
