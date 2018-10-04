using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	/// <summary>
	/// Služba pro získávání primárního klíče modelových objektů.
	/// </summary>
	public interface IEntityKeyAccessor<TEntity, TEntityKey>
		where TEntity : class
	{
		/// <summary>
		/// Vrátí hodnotu primárního klíče entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		TEntityKey GetEntityKey(TEntity entity);

		/// <summary>
		/// Vrátí název vlastnosti, která je primárním klíčem.
		/// </summary>
		string GetEntityKeyPropertyName();
	}
}