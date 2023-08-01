namespace Havit.Data.EntityFrameworkCore;

/// <summary>
/// Factory pro vytvoření instance IDbContextu.
/// </summary>
public interface IDbContextFactory
{
	/// <summary>
	/// Vytvoří novou instanci IDbContext. Je zodpovědností volajícího, aby provedl úklid instance (dispose)!
	/// </summary>
	IDbContext CreateDbContext();
}
