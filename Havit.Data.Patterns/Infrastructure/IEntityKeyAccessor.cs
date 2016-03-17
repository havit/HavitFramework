using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	/// <summary>
	/// Třída pro získávání identifikátoru modelových objektů.
	/// </summary>
	public interface IEntityKeyAccessor<TEntity, TEntityKey>
		where TEntity : class, ILanguage
	{
		/// <summary>
		/// Vrátí klíč entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		TEntityKey GetEntityKey(TEntity entity);
	}
}