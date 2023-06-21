using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;

/// <summary>
/// Validátor, který se spustí před provedením Commitu na UoW.
/// </summary>
public interface IEntityValidator<in TEntity>
{
	/// <summary>
	/// Template metoda pro provedení validace entity před Commitem na UoW.
	/// </summary>
	/// <param name="changeType">Prováděná operace s entitou (Insert/Update/Delete).</param>
	/// <param name="changingEntity">Entita, nad níž bude operace provedena.</param>
	/// <returns>Seznam detekovaných chyb.</returns>
	IEnumerable<string> Validate(ChangeType changeType, TEntity changingEntity);
}
